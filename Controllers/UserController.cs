using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using quiz_project.Common;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Extensions;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class UserController(QuizDb context, SignInManager<User> signInManager,
        UserManager<User> userManager, IUserService userService) : Controller
    {
        [HttpGet]
        public async Task<ViewResult> Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<ViewResult> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            return View();
        }

        [HttpGet, ActionName("Settings")]
        public async Task<ViewResult> AccountSettings()
        {
            return View();
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Register()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await userService.RegisterAsync(registerViewModel);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View(registerViewModel);
                }
                return RedirectToAction("BrowseCourses", "Menu");
            }

            return View(registerViewModel);
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (ModelState.IsValid)
            {
                var (success, error) = await userService.LoginAsync(loginViewModel);
                if (!success)
                {
                    ModelState.AddModelError(string.Empty, error);
                    return View(loginViewModel);
                }

                var user = await userManager.FindByNameAsync(loginViewModel.UserName);
                if (user is not null && await userManager.IsInRoleAsync(user, "Creator"))
                    return RedirectToAction("Index", "Course");
                if (user is not null && await userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("Users", "Admin");

                return RedirectToAction("BrowseCourses", "Menu");
            }

            return View(loginViewModel);
        }

        [HttpGet]
        public async Task<RedirectToActionResult> Logout()
        {
            await userService.LogoutAsync(User);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeViewModel passwordChangeViewModel)
        {
            var (success, error) = await userService.ChangePasswordAsync(User.Identity.Name, passwordChangeViewModel);
            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Password changed successfully.";
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(EmailChangeViewModel emailChangeViewModel)
        {
            var (success, error) = await userService.ChangeEmailAsync(User.Identity.Name, emailChangeViewModel);
            if (!success)
                TempData["ErrorMessage"] = error;
            else
                TempData["SuccessMessage"] = "Email changed successfully.";
            return RedirectToAction("Settings");
        }

        [HttpGet, AllowAnonymous, ActionName("RegisterCreator")]
        public IActionResult RegisterCreatorGet() => View();

        [HttpPost, AllowAnonymous, ActionName("RegisterCreator")]
        public async Task<IActionResult> RegisterCreatorPost(RegisterCreatorViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var (success, error) = await userService.RegisterCreatorAsync(vm);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                return View(vm);
            }

            return View("CreatorApplicationSent");
        }
    }
}