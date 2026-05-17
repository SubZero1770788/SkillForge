using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.Services
{
    public class QuizGameService(
        IQuizRepository quizRepository,
        UserManager<User> userManager,
        IQuizMapper quizMapper,
        IOnGoingQuizRepository onGoingQuizRepository,
        IAttemptRepository attemptRepository,
        IAccessValidationService accessValidationService,
        IQuizQueryService quizQueryService,
        IPaginationService<Question> paginationService) : IQuizGameService
    {
        public async Task<(bool, QuizSummaryViewModel?)> AttemptSummary(int quizId, User user)
        {
            var quizMetaData = await onGoingQuizRepository.GetAsync(user.Id, quizId);
            var userRole = await userManager.GetRolesAsync(user);

            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !userRole.Contains("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return (false, null);
            }

            var allQuizAttempts = await attemptRepository.GetAllAttemptsAsync(quizId);
            var quizDefinition = await quizRepository.GetQuizByIdAsync(quizId);

            var latestplayerScore = await attemptRepository.GetLatestUserAttemptAsync(user.Id);
            var bestPlayerScore = await attemptRepository.GetTopUserAttemptAsync(user.Id, quizId);

            var topScores = allQuizAttempts.OrderByDescending(aqa => aqa.Score).Take(10).ToList();
            var users = await userManager.Users.ToDictionaryAsync(u => u.Id, u => u.UserName);

            var quizSummaryViewModel = quizMapper.ToQuizSummaryViewModel(quizDefinition, topScores, users, latestplayerScore, bestPlayerScore);

            return (true, quizSummaryViewModel);
        }

        public async Task<QuizViewModel> LaunchQuizAsync(int quizId)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            var quizViewModel = quizMapper.ToQuizViewModel(quiz);

            return quizViewModel;
        }

        public async Task StartQuizAsync(int quizId, int userId)
        {
            var allQuestions = await quizRepository.GetQuestionsByQuizId(quizId);
            var isQuizRandomized = await quizQueryService.CheckIfRandomizedAsync(quizId);

            var selectedQuestions = isQuizRandomized
                ? allQuestions.OrderBy(x => Guid.NewGuid())
                : allQuestions.OrderBy(x => x.QuestionId);

            var onGoingQuizQuestions = selectedQuestions
                .Take(20)
                .Select((question, index) => new OnGoingQuizQuestion
                {
                    QuestionId = question.QuestionId,
                    Order = index
                })
                .ToList();

            var state = new OnGoingQuizState
            {
                QuizId = quizId,
                UserId = userId,
                CurrentPage = 1,
                QuestionCount = 5,
                IsRandomized = isQuizRandomized,
                Questions = onGoingQuizQuestions
            };

            await onGoingQuizRepository.DeleteAsync(userId, quizId);
            await onGoingQuizRepository.CreateOrUpdateAsync(state);
        }

        public async Task<(bool Success, GameViewModel? GameViewModel)> GetPlayAsync(int quizId, int userId)
        {
            var quizMetaData = await onGoingQuizRepository.GetAsync(userId, quizId);
            if (quizMetaData is null) return (false, null);

            var orderedQuestions = await GetOrderedQuestionsAsync(quizId, quizMetaData);

            var paginatedQuestions = await paginationService.GetPagedDataAsync(
                orderedQuestions,
                quizMetaData.CurrentPage,
                quizMetaData.QuestionCount
            );

            if (!paginatedQuestions.Any()) return (false, null);

            var gameViewModel = ToGameViewModel(
                quizMetaData,
                paginatedQuestions,
                paginatedQuestions.TotalPages
            );

            return (true, gameViewModel);
        }

        public async Task<(bool Finished, GameViewModel? GameViewModel)> SubmitPlayAsync(GameViewModel model, int userId)
        {
            var quizMetaData = await onGoingQuizRepository.GetAsync(userId, model.QuizId);
            if (quizMetaData is null) return (true, null);

            SaveAnswersFromCurrentPage(model, quizMetaData);

            await onGoingQuizRepository.CreateOrUpdateAsync(quizMetaData);

            var orderedQuestions = await GetOrderedQuestionsAsync(model.QuizId, quizMetaData);

            var paged = await paginationService.GetPagedDataAsync(
                orderedQuestions,
                quizMetaData.CurrentPage,
                quizMetaData.QuestionCount
            );

            if (!paged.Any())
            {
                await FinishAttemptAsync(model.QuizId, userId, quizMetaData, orderedQuestions);
                return (true, null);
            }

            var gameViewModel = ToGameViewModel(
                quizMetaData,
                paged,
                paged.TotalPages
            );

            return (false, gameViewModel);
        }

        private async Task<List<Question>> GetOrderedQuestionsAsync(int quizId, OnGoingQuizState quizMetaData)
        {
            var questionIds = quizMetaData.Questions
                .OrderBy(x => x.Order)
                .Select(x => x.QuestionId)
                .ToList();

            var allQuestions = await quizRepository.GetQuestionsByQuizId(quizId);

            return questionIds
                .Select(id => allQuestions.First(q => q.QuestionId == id))
                .ToList();
        }

        private static void SaveAnswersFromCurrentPage(GameViewModel model, OnGoingQuizState quizMetaData)
        {
            var currentUserAnswers = model.Questions
                .Select(q => new AnswerState
                {
                    QuestionId = q.QuestionId,
                    AnswersId = q.SelectedAnswerIds ?? new List<int>(),
                    OnGoingQuizStateId = quizMetaData.Id
                })
                .ToList();

            foreach (var answer in currentUserAnswers)
            {
                var existing = quizMetaData.Answers
                    .FirstOrDefault(a => a.QuestionId == answer.QuestionId);

                if (existing != null)
                {
                    existing.AnswersId = answer.AnswersId;
                }
                else
                {
                    quizMetaData.Answers.Add(answer);
                }
            }
        }

        private async Task FinishAttemptAsync(
            int quizId,
            int userId,
            OnGoingQuizState quizMetaData,
            List<Question> questions)
        {
            var correctAnswersByQuestion = questions.ToDictionary(
                q => q.QuestionId,
                q => q.Answers
                    .Where(a => a.IsCorrect)
                    .Select(a => a.AnswerId)
                    .OrderBy(id => id)
                    .ToList()
            );

            var questionScoreById = questions.ToDictionary(
                q => q.QuestionId,
                q => q.QuestionScore
            );

            int userScore = quizMetaData.Answers.Sum(userAnswer =>
            {
                if (!correctAnswersByQuestion.TryGetValue(userAnswer.QuestionId, out var correctAnswerIds))
                    return 0;

                var userAnswerIds = userAnswer.AnswersId
                    .OrderBy(id => id)
                    .ToList();

                return correctAnswerIds.SequenceEqual(userAnswerIds)
                    ? questionScoreById[userAnswer.QuestionId]
                    : 0;
            });

            var attempt = new QuizAttempt
            {
                UserId = userId,
                QuizId = quizId,
                Score = userScore,
                AnswerSelections = quizMetaData.Answers
                    .SelectMany(ua => ua.AnswersId.Select(answerId => new AnswerSelection
                    {
                        QuestionId = ua.QuestionId,
                        AnswerId = answerId,
                        IsCorrect = correctAnswersByQuestion.TryGetValue(ua.QuestionId, out var correctIds)
                                    && correctIds.Contains(answerId)
                    }))
                    .ToList()
            };

            await onGoingQuizRepository.DeleteAsync(userId, quizId);
            await attemptRepository.CreateAsync(attempt);
        }

        private static GameViewModel ToGameViewModel(
            OnGoingQuizState quizMetaData,
            IEnumerable<Question> questions,
            int totalPages)
        {
            return new GameViewModel
            {
                QuizId = quizMetaData.QuizId,
                CurrentPage = quizMetaData.CurrentPage,
                TotalPages = totalPages,
                Questions = questions.Select(question => new QuestionViewModel
                {
                    QuestionId = question.QuestionId,
                    Description = question.Description,
                    QuestionScore = question.QuestionScore,
                    Answers = question.Answers.Select(ans => new AnswerViewModel
                    {
                        AnswerId = ans.AnswerId,
                        Description = ans.Description,
                        IsCorrect = false
                    }).ToList()
                }).ToList()
            };
        }
    }
}