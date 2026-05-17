using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class ModuleController(IModuleService moduleService, ICourseQueryService courseQueryService,
        IAccessValidationService accessValidationService, IQuizRepository quizRepository,
        UserManager<User> userManager) : Controller
    {
        private async Task LoadAvailableQuizzes(int userId)
        {
            var quizzes = await quizRepository.GetQuizesByUserAsync(userId);
            ViewBag.AvailableQuizzes = quizzes
                .Where(q => q.Scope == QuizScope.Module)
                .Select(q => new { q.QuizId, q.Title })
                .ToList();
        }

        [HttpGet, ActionName("Create")]
        public async Task<IActionResult> CreateModuleAsync(int courseId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(courseId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await LoadAvailableQuizzes(user.Id);
            return View(new ModuleViewModel { CourseId = courseId });
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreateModuleAsync(ModuleViewModel moduleViewModel)
        {
            if (!ModelState.IsValid)
            {
                var user2 = await userManager.GetUserAsync(User);
                await LoadAvailableQuizzes(user2!.Id);
                return View(moduleViewModel);
            }

            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(moduleViewModel.CourseId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            var (success, error) = await moduleService.CreateAsync(moduleViewModel);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                await LoadAvailableQuizzes(user.Id);
                return View(moduleViewModel);
            }

            return RedirectToAction("Edit", "Course", new { courseId = moduleViewModel.CourseId });
        }

        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditModuleAsync(int moduleId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            var moduleViewModel = await moduleService.GetEditAsync(moduleId);
            if (moduleViewModel is null) return RedirectToAction("Index", "Course");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(moduleViewModel.CourseId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await LoadAvailableQuizzes(user.Id);
            return View(moduleViewModel);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditModuleAsync(ModuleViewModel moduleViewModel)
        {
            if (!ModelState.IsValid)
            {
                var user2 = await userManager.GetUserAsync(User);
                await LoadAvailableQuizzes(user2!.Id);
                return View(moduleViewModel);
            }

            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(moduleViewModel.CourseId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            var (success, errors) = await moduleService.PostEditAsync(moduleViewModel);
            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                await LoadAvailableQuizzes(user.Id);
                return View(moduleViewModel);
            }

            return RedirectToAction("Edit", "Course", new { courseId = moduleViewModel.CourseId });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteModuleAsync(int moduleId, int courseId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(courseId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await moduleService.DeleteAsync(moduleId);
            return RedirectToAction("Edit", "Course", new { courseId });
        }

        [HttpPost, ActionName("Reorder")]
        public async Task<IActionResult> ReorderModulesAsync(int courseId, List<ReorderItem> items)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsCourseAsync(courseId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await moduleService.ReorderAsync(items);
            return RedirectToAction("Edit", "Course", new { courseId });
        }
    }
}
