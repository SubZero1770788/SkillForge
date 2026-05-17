using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class ChapterController(IChapterService chapterService, IAccessValidationService accessValidationService,
        IQuizRepository quizRepository, UserManager<User> userManager) : Controller
    {
        private async Task LoadAvailableQuizzes(int userId)
        {
            var quizzes = await quizRepository.GetQuizesByUserAsync(userId);
            ViewBag.AvailableQuizzes = quizzes
                .Select(q => new { q.QuizId, q.Title, q.Scope })
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> View(int chapterId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            var chapterViewModel = await chapterService.GetViewAsync(chapterId, user.Id);
            if (chapterViewModel is null) return RedirectToAction("Index", "Course");

            if (chapterViewModel.IsLocked)
                return View("Locked", chapterViewModel);

            return View(chapterViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(int chapterId, int moduleId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            await chapterService.MarkAsCompletedAsync(chapterId, user.Id);
            return RedirectToAction("View", new { chapterId });
        }

        [HttpGet, ActionName("Create")]
        public async Task<IActionResult> CreateChapterAsync(int moduleId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsModuleAsync(moduleId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await LoadAvailableQuizzes(user.Id);
            return View(new ChapterViewModel { ModuleId = moduleId });
        }

        [HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreateChapterAsync(ChapterViewModel chapterViewModel)
        {
            if (!ModelState.IsValid)
            {
                var user2 = await userManager.GetUserAsync(User);
                await LoadAvailableQuizzes(user2!.Id);
                return View(chapterViewModel);
            }

            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsModuleAsync(chapterViewModel.ModuleId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            var (success, error) = await chapterService.CreateAsync(chapterViewModel);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, error);
                await LoadAvailableQuizzes(user.Id);
                return View(chapterViewModel);
            }

            return RedirectToAction("Edit", "Module", new { moduleId = chapterViewModel.ModuleId });
        }

        [HttpGet, ActionName("Edit")]
        public async Task<IActionResult> EditChapterAsync(int chapterId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            var chapterViewModel = await chapterService.GetEditAsync(chapterId);
            if (chapterViewModel is null) return RedirectToAction("Index", "Course");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsModuleAsync(chapterViewModel.ModuleId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await LoadAvailableQuizzes(user.Id);
            return View(chapterViewModel);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditChapterAsync(ChapterViewModel chapterViewModel)
        {
            if (!ModelState.IsValid)
            {
                var user2 = await userManager.GetUserAsync(User);
                await LoadAvailableQuizzes(user2!.Id);
                return View(chapterViewModel);
            }

            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsModuleAsync(chapterViewModel.ModuleId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            var (success, errors) = await chapterService.PostEditAsync(chapterViewModel);
            if (!success)
            {
                foreach (var error in errors)
                    ModelState.AddModelError(string.Empty, error);
                await LoadAvailableQuizzes(user.Id);
                return View(chapterViewModel);
            }

            return RedirectToAction("Edit", "Module", new { moduleId = chapterViewModel.ModuleId });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteChapterAsync(int chapterId, int moduleId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsModuleAsync(moduleId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await chapterService.DeleteAsync(chapterId);
            return RedirectToAction("Edit", "Module", new { moduleId });
        }

        [HttpPost, ActionName("Reorder")]
        public async Task<IActionResult> ReorderChaptersAsync(int moduleId, List<ReorderItem> items)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsModuleAsync(moduleId, user);
                if (!owns) return RedirectToAction("Index", "Course");
            }

            await chapterService.ReorderAsync(items);
            return RedirectToAction("Edit", "Module", new { moduleId });
        }
    }
}
