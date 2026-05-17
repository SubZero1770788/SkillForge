using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Controllers
{
    public class MenuController(IQuizQueryService quizQueryService, ICourseQueryService courseQueryService,
        IUserService userService, UserManager<User> userManager) : Controller
    {
        [HttpGet, ActionName("Browse")]
        public async Task<IActionResult> BrowsePublicQuizzes()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            var quizViewModels = await quizQueryService.GetPublicQuizzesAsync();
            return View(quizViewModels);
        }

        [HttpGet, ActionName("BrowseCourses")]
        public async Task<IActionResult> BrowsePublicCourses()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            var courseViewModels = await courseQueryService.GetPublicCoursesAsync();
            return View(courseViewModels);
        }

        [HttpGet, ActionName("Active")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            var users = await userService.GetAllActiveUsersAsync();
            return View(users);
        }
    }
}
