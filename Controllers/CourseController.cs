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

    public class CourseController(ICourseQueryService courseQueryService, ICourseService courseService,
    IAccessValidationService accessValidationService, UserManager<User> userManager) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var courseViewModels = await courseQueryService.GetUserCoursesAsync(user.Id);

            return View(courseViewModels);
        }

        // [HttpGet]
        // public async Task<IActionResult> GetQuizAsync(int quizId)
        // {
        //     var user = await userManager.GetUserAsync(User);
        //     if (user is null) return RedirectToAction("Register", "User")!;

        //     var quiz = await quizRepository.GetQuizByIdAsync(quizId);
        //     var quizModel = new QuizViewModel
        //     {
        //         Title = quiz.Title,
        //         Description = quiz.Description
        //     };

        //     return View(quizModel);
        // }

        // [HttpGet, ActionName("Statistics")]
        // public async Task<IActionResult> CheckQuizStats(int Id)
        // {
        //     var user = await userManager.GetUserAsync(User);
        //     if (user is null) return RedirectToAction("Register", "User")!;

        //     if (!await quizQueryService.CheckIfPublicAsync(Id) && !User.IsInRole("Admin"))
        //     {
        //         var owns = await accessValidationService.UserOwnsQuizAsync(Id, user);
        //         if (!owns) return RedirectToAction("Index")!;
        //     }

        //     var (success, quizStatisticsModel) = await quizQueryService.GetQuizStatisticsAsync(Id, user.Id);
        //     if (!success) return View("ZeroAttempts");

        //     return View(quizStatisticsModel);
        // }

        [HttpGet, ActionName("Create")]
        public async Task<IActionResult> CreateNewCourseAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            return View();
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreateNewQuizAsync(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user is null) return RedirectToAction("Register", "User")!;

                var (success, error) = await courseService.CreateAsync(courseViewModel, user.Id);
                if (!success)
                {
                    ModelState.AddModelError(String.Empty, error);
                    return View(courseViewModel);
                }

                return RedirectToAction("Index");
            }
            return View(courseViewModel);
        }

        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditCourseAsync(int courseId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await courseQueryService.CheckIfPublicAsync(courseId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(courseId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var courseViewModel = await courseService.GetEditAsync(courseId);

            return View(courseViewModel);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditCourseAsync(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user is null) return RedirectToAction("Register", "User")!;
                if (!await courseQueryService.CheckIfPublicAsync(courseViewModel.CourseId) && !User.IsInRole("Admin"))
                {
                    var owns = await accessValidationService.UserOwnsCourseAsync(courseViewModel.CourseId, user);
                    if (!owns) return RedirectToAction("Index")!;
                }

                var (success, errors) = await courseService.PostEditAsync(courseViewModel, user.Id);

                if (!success)
                {
                    foreach (var er in errors)
                    {
                        ModelState.AddModelError(String.Empty, er);
                    }
                    return View(courseViewModel);
                }

                return RedirectToAction("Index");

            }
            return RedirectToAction("Edit");
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteCourseAsync(int Id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await courseQueryService.CheckIfPublicAsync(Id) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(Id, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            try
            {
                await courseService.DeleteAsync(Id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, $"Something went wrong: {e}");
            }

            return RedirectToAction("Index");
        }

        // [HttpGet, ActionName("Game")]
        // public async Task<IActionResult> LaunchCourseAsync(int courseId)
        // {
        //     var user = await userManager.GetUserAsync(User);
        //     if (user is null) return RedirectToAction("Register", "User")!;
        //     if (!await quizQueryService.CheckIfPublicAsync(quizId) && !User.IsInRole("Admin"))
        //     {
        //         var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
        //         if (!owns) return RedirectToAction("Index")!;
        //     }

        //     var quizViewModel = await quizGameService.LaunchQuizAsync(quizId);

        //     return View(quizViewModel);
        // }
    }
}