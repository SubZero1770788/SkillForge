using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class QuizService(IQuizRepository quizRepository, IQuizMapper quizMapper,
        IAccessValidationService accessValidationService, IAttemptRepository attemptRepository,
        IFileStorageService fileStorageService) : IQuizService
    {
        private async Task DeleteQuizImagesAsync(IEnumerable<Question> questions)
        {
            var tasks = questions
                .Where(q => !string.IsNullOrEmpty(q.ImagePath))
                .Select(q => fileStorageService.DeleteAsync(q.ImagePath!));
            await Task.WhenAll(tasks);
        }
        public async Task<(bool success, string error)> CreateAsync(QuizViewModel quizViewModel, int userId)
        {
            var errors = accessValidationService.EachQuestionHasAnswer(quizViewModel).ToList();
            if (errors.Count > 0)
                return (false, errors.First());

            if (quizViewModel.IsPublic && (quizViewModel.TotalScore < 50 || quizViewModel.QuestionCount < 5))
                return (false, "A public quiz needs at least 5 questions with 50 combined points.");

            if (quizViewModel.IsPublic && await quizRepository.PublicTitleExistsAsync(quizViewModel.Title))
                return (false, $"A public quiz named \"{quizViewModel.Title}\" already exists. Choose a different title.");

            var quiz = quizMapper.ToEntity(quizViewModel, userId);
            await quizRepository.CreateQuizAsync(quiz);
            return (true, string.Empty);
        }

        public async Task<(bool success, string error)> DeleteAsync(int quizId)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            if (quiz is null) return (false, "Quiz not found.");
            await DeleteQuizImagesAsync(quiz.Questions);
            await quizRepository.DeleteQuizAsync(quiz);
            return (true, string.Empty);
        }

        public async Task<QuizViewModel> GetEditAsync(int quizId)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            var quizViewModel = quizMapper.ToQuizViewModel(quiz);
            return quizViewModel;
        }

        public async Task<(bool success, IEnumerable<string> errors)> PostEditAsync(QuizViewModel quizViewModel, int userId)
        {
            if (await HasAttemptsAsync(quizViewModel.QuizId))
                return (false, new[] { "This quiz has recorded attempts and cannot be edited. Duplicate it to create an editable copy." });

            var errors = accessValidationService.EachQuestionHasAnswer(quizViewModel).ToList();

            if (errors.Count() > 0)
                return (false, errors);

            if (quizViewModel.IsPublic && (quizViewModel.TotalScore < 50 || quizViewModel.QuestionCount < 5))
            {
                return (false, new[]
                {
                    "In order for quiz to be public it needs at least 5 questions with 50 combined points"
                });
            }

            if (quizViewModel.IsPublic && await quizRepository.PublicTitleExistsAsync(quizViewModel.Title, quizViewModel.QuizId))
                return (false, new[] { $"A public quiz named \"{quizViewModel.Title}\" already exists. Choose a different title." });

            var quiz = quizMapper.ToEntity(quizViewModel, userId);

            await quizRepository.UpdateQuizAsync(quiz);
            return (true, Enumerable.Empty<string>());
        }

        public async Task<bool> HasAttemptsAsync(int quizId)
            => (await attemptRepository.GetAllAttemptsAsync(quizId)).Any();

        public async Task<(bool success, string error)> DuplicateAsync(int quizId, int userId)
        {
            var original = await quizRepository.GetQuizByIdAsync(quizId);
            if (original is null) return (false, "Quiz not found.");

            var questions = await quizRepository.GetQuestionsByQuizId(quizId);

            var copy = new Quiz
            {
                Title = original.Title + " (copy)",
                Description = original.Description,
                IsPublic = false,
                IsRandomized = original.IsRandomized,
                Scope = QuizScope.Standalone,
                PassPercentage = original.PassPercentage,
                UserId = userId,
                Questions = questions.Select(q => new Question
                {
                    Description = q.Description,
                    QuestionScore = q.QuestionScore,
                    Type = q.Type,
                    Grading = q.Grading,
                    Keywords = q.Keywords,
                    ImagePath = q.ImagePath,
                    Answers = q.Answers.Select(a => new Answer
                    {
                        Description = a.Description,
                        IsCorrect = a.IsCorrect
                    }).ToList()
                }).ToList()
            };

            await quizRepository.CreateQuizAsync(copy);
            return (true, string.Empty);
        }
    }
}