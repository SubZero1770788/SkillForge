using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Extensions;
using quiz_project.Interfaces;
using quiz_project.RuntimeModels;
using quiz_project.Services;
using quiz_project.ViewModels;
using static quiz_project.RuntimeModels.QuizMetaData;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.Controllers
{
    public class QuizGameController(IAccessValidationService accessValidationService, IPaginationService<Question> paginationService,
                                        IQuizRepository quizRepository, IAttemptRepository attemptRepository, UserManager<User> userManager,
                                        IQuizGameService quizGameService, IOnGoingQuizRepository onGoingQuizRepository, IQuizQueryService quizQueryService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(int QuizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(QuizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(QuizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            // needs a service
            var state = new OnGoingQuizState
            {
                QuizId = QuizId,
                UserId = user.Id,
                CurrentPage = 1,
                QuestionCount = 5
            };


            await onGoingQuizRepository.CreateOrUpdateAsync(state);
            // till there

            return RedirectToAction("Play", new { QuizId = QuizId });
        }

        [HttpGet, ActionName("Play")]
        public async Task<IActionResult> Play(int quizId)
        {
            // fix this to include the quizMetaData instead of controller, also redirects ONLY from controllers
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quizMetaData = await onGoingQuizRepository.GetAsync(user.Id, quizId);
            // Either you're during a quiz or you shouldn't be there...

            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            List<Question> questions = await quizRepository.GetQuestionsByQuizId(quizId);

            var paginatedQuestiuons = await paginationService.GetPagedDataAsync(questions, quizMetaData.CurrentPage, quizMetaData.QuestionCount);

            if (!paginatedQuestiuons.Any())
            {
                // Return something if Quiz is empty...
                return View("Empty");
            }

            // TotalPages should find it's way into QuizMetaData
            var gameViewModel = new GameViewModel
            {
                QuizId = quizId,
                CurrentPage = quizMetaData.CurrentPage,
                TotalPages = paginatedQuestiuons.TotalPages,
                Questions = paginatedQuestiuons.Select(question => new QuestionViewModel
                {
                    QuestionId = question.QuestionId,
                    Description = question.Description,
                    QuestionScore = question.QuestionScore,
                    Answers = question.Answers.Select(ans => new AnswerViewModel
                    {
                        // This is done this way in order to not reveal the answer to question
                        AnswerId = ans.AnswerId,
                        Description = ans.Description,
                        IsCorrect = false
                    }).ToList()
                }).ToList()
            };


            return View(gameViewModel);
        }

        [HttpPost, ActionName("Play")]
        public async Task<IActionResult> Play(GameViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            // Either you're during a quiz or you shouldn't be there...
            var quizMetaData = await onGoingQuizRepository.GetAsync(user.Id, model.QuizId);

            if (!await quizQueryService.CheckIfPublicAsync(model.QuizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(model.QuizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            List<AnswerState> currentUserAnswers = model.Questions
                .Select(q => new AnswerState
                {
                    QuestionId = q.QuestionId,
                    AnswersId = model.Questions
                        .FirstOrDefault(mq => mq.QuestionId == q.QuestionId)?
                        .SelectedAnswerIds ?? new List<int>()
                }).ToList();

            foreach (var answer in currentUserAnswers)
            {
                answer.OnGoingQuizStateId = quizMetaData.Id;

                var existing = quizMetaData.Answers.FirstOrDefault(a => a.QuestionId == answer.QuestionId);
                if (existing != null)
                {
                    existing.AnswersId = answer.AnswersId;
                }
                else
                {
                    quizMetaData.Answers.Add(answer);
                }
            }
            await onGoingQuizRepository.CreateOrUpdateAsync(quizMetaData);

            var questionQuery = await quizRepository.GetQuestionsByQuizId(model.QuizId);
            var paged = await paginationService.GetPagedDataAsync(questionQuery, quizMetaData.CurrentPage, quizMetaData.QuestionCount);
            var previousPage = await paginationService.GetPagedDataAsync(questionQuery, quizMetaData.CurrentPage - 1, quizMetaData.QuestionCount);

            if (!paged.Any())
            {
                var correctAnswersByQuestion = questionQuery.ToDictionary(q => q.QuestionId, q => q.Answers.Where(a => a.IsCorrect)
                                                                .Select(a => a.AnswerId).OrderBy(id => id).ToList());

                int userScore = quizMetaData.Answers.Sum(q =>
                {
                    if (!correctAnswersByQuestion.TryGetValue(q.QuestionId, out var correctAnswerIds))
                        return 0;

                    var userAnswerIds = q.AnswersId.OrderBy(id => id).ToList();
                    return correctAnswerIds.SequenceEqual(userAnswerIds) ? questionQuery.Where(q => q.QuestionId == q.QuestionId).First().QuestionScore : 0;
                });

                var attempt = new QuizAttempt
                {
                    UserId = user.Id,
                    QuizId = model.QuizId,
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

                await onGoingQuizRepository.DeleteAsync(quizMetaData.QuizId, user.Id);
                await attemptRepository.CreateAsync(attempt);

                return RedirectToAction("Summary", new { QuizId = quizMetaData.QuizId });
            }

            var gameViewModel = new GameViewModel
            {
                QuizId = quizMetaData.QuizId,
                CurrentPage = quizMetaData.CurrentPage,
                TotalPages = paged.TotalPages,
                Questions = paged.Select(question => new QuestionViewModel
                {
                    QuestionId = question.QuestionId,
                    Description = question.Description,
                    QuestionScore = question.QuestionScore,
                    Answers = question.Answers.Select(ans => new AnswerViewModel
                    {
                        // This is done this way in order to not reveal the answer to question
                        AnswerId = ans.AnswerId,
                        Description = ans.Description,
                        IsCorrect = false
                    }).ToList()
                }).ToList()
            };

            return View(gameViewModel);
        }

        [HttpGet, ActionName("Summary")]
        public async Task<IActionResult> QuizAttemptSummary(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var (success, quizSummaryViewModel) = await quizGameService.AttemptSummary(quizId, user);
            if (!success) return RedirectToAction("Index");

            return View(quizSummaryViewModel);
        }
    }
}