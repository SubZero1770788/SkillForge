using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class ChapterController(IChapterService chapterService, IAccessValidationService accessValidationService,
        IQuizRepository quizRepository, IEnrollmentService enrollmentService,
        IModuleRepository moduleRepository, IChapterRepository chapterRepository,
        IFileStorageService fileStorageService, UserManager<User> userManager) : Controller
    {
        private async Task LoadAvailableQuizzes(int userId)
        {
            var quizzes = await quizRepository.GetQuizesByUserAsync(userId);
            ViewBag.AvailableQuizzes = quizzes
                .Where(q => q.Scope == QuizScope.Chapter)
                .Select(q => new { q.QuizId, q.Title })
                .ToList();
        }

        [HttpGet]
        public async Task<IActionResult> View(int chapterId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            if (!User.IsInRole("Admin") && !User.IsInRole("Creator"))
            {
                var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
                if (chapter is not null)
                {
                    var module = await moduleRepository.GetModuleByIdAsync(chapter.ModuleId);
                    if (module is not null)
                    {
                        var status = await enrollmentService.GetStatusAsync(module.CourseId, user.Id);
                        if (status != EnrollmentStatus.Approved)
                            return RedirectToAction("Details", "Course", new { courseId = module.CourseId });
                    }
                }
            }

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
        public async Task<IActionResult> CreateChapterAsync(ChapterViewModel chapterViewModel, IFormFile? file)
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

            if (file is not null && file.Length > 0)
            {
                var chapter = (await chapterRepository.GetChaptersByModuleIdAsync(chapterViewModel.ModuleId))
                    .OrderByDescending(c => c.ChapterId).First();
                var objectKey = await fileStorageService.UploadAsync(file, chapter.ChapterId);
                chapterViewModel.ChapterId = chapter.ChapterId;
                chapterViewModel.FilePath = objectKey;
                await chapterService.PostEditAsync(chapterViewModel);
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
        public async Task<IActionResult> EditChapterAsync(ChapterViewModel chapterViewModel, IFormFile? file)
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

            if (file is not null && file.Length > 0)
            {
                if (!string.IsNullOrEmpty(chapterViewModel.FilePath))
                    await fileStorageService.DeleteAsync(chapterViewModel.FilePath);

                chapterViewModel.FilePath = await fileStorageService.UploadAsync(file, chapterViewModel.ChapterId);
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

        [HttpGet, ActionName("Download")]
        public async Task<IActionResult> DownloadFileAsync(int chapterId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null || string.IsNullOrEmpty(chapter.FilePath))
                return NotFound();

            var url = await fileStorageService.GetPresignedUrlAsync(chapter.FilePath);
            return Redirect(url);
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
