using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Database.Migrations;
using quiz_project.Database.Repositories;
using quiz_project.Entities;
using quiz_project.Entities.Repositories;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.Controllers
{

    public class QuizController(IQuizRepository quizRepository, IAccessValidationService accessValidationService,
                                 UserManager<User> userManager, IQuizService quizService, IQuizGameService quizGameService,
                                 IQuizQueryService quizQueryService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quizViewModels = await quizQueryService.GetUserQuizzesAsync(user.Id);

            return View(quizViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            var quizModel = new QuizViewModel
            {
                Title = quiz.Title,
                Description = quiz.Description
            };

            return View(quizModel);
        }

        [HttpGet, ActionName("Statistics")]
        public async Task<IActionResult> CheckQuizStats(int Id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            if (!await quizQueryService.CheckIfPublicAsync(Id) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(Id, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var (success, quizStatisticsModel) = await quizQueryService.GetQuizStatisticsAsync(Id, user.Id);
            if (!success) return View("ZeroAttempts");

            return View(quizStatisticsModel);
        }

        [HttpGet, ActionName("Create")]
        public async Task<IActionResult> CreateNewQuizAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            return View();
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreateNewQuizAsync(QuizViewModel quizViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user is null) return RedirectToAction("Register", "User")!;

                var (success, error) = await quizService.CreateAsync(quizViewModel, user.Id);
                if (!success)
                {
                    ModelState.AddModelError(String.Empty, error);
                    return View(quizViewModel);
                }

                return RedirectToAction("Index");
            }
            return View(quizViewModel);
        }

        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var quizViewModel = await quizService.GetEditAsync(quizId);

            return View(quizViewModel);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditQuizAsync(QuizViewModel quizViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user is null) return RedirectToAction("Register", "User")!;
                if (!await quizQueryService.CheckIfPublicAsync(quizViewModel.QuizId) && !User.IsInRole("Admin"))
                {
                    var owns = await accessValidationService.UserOwnsQuizAsync(quizViewModel.QuizId, user);
                    if (!owns) return RedirectToAction("Index")!;
                }

                var (success, errors) = await quizService.PostEditAsync(quizViewModel, user.Id);

                if (!success)
                {
                    foreach (var er in errors)
                    {
                        ModelState.AddModelError(String.Empty, er);
                    }
                    return View(quizViewModel);
                }

                return RedirectToAction("Index");

            }
            return RedirectToAction("Edit");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteQuizAsync(int Id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(Id) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(Id, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            try
            {
                await quizService.DeleteAsync(Id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, $"Something went wrong: {e}");
            }

            return RedirectToAction("Index");
        }

        [HttpGet, ActionName("Game")]
        public async Task<IActionResult> LaunchQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var quizViewModel = await quizGameService.LaunchQuizAsync(quizId);

            return View(quizViewModel);
        }
    }
}