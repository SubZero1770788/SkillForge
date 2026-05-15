using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class MenuController(IQuizRepository quizRepository, UserManager<User> userManager, IUserService userService) : Controller
    {
        [HttpGet, ActionName("Browse")]
        public async Task<IActionResult> BrowsePublicQuizzes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quizes = await quizRepository.GetPublicQuizzes();

            var quizViewModels = new List<QuizViewModel>();
            foreach (Quiz q in quizes)
            {
                var quizViewModel = new QuizViewModel
                {
                    QuizId = q!.QuizId,
                    Title = q.Title,
                    Description = q.Description,
                    Questions = q.Questions.Select(qvm => new QuestionViewModel
                    {
                        QuestionScore = qvm.QuestionScore,
                        Description = qvm.Description,
                        Answers = qvm.Answers.Select(avm => new AnswerViewModel
                        {
                            Description = avm.Description,
                            IsCorrect = avm.IsCorrect
                        }).ToList()
                    }).ToList()
                };
                quizViewModels.Add(quizViewModel);
            }

            return View(quizViewModels);
        }

        [HttpGet, ActionName("Active")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            var users = await userService.GetAllActiveUsersAsync();
            return View(users);
        }

    }
}