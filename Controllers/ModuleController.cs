using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class ModuleController(IModuleService moduleService, ICourseQueryService courseQueryService,
        IAccessValidationService accessValidationService, IQuizRepository quizRepository,
        IModuleRepository moduleRepository, IFileStorageService fileStorageService,
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
        public async Task<IActionResult> CreateModuleAsync(ModuleViewModel moduleViewModel, IFormFile? file)
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

            if (file is not null && file.Length > 0)
            {
                var modules = await moduleRepository.GetModulesByCourseIdAsync(moduleViewModel.CourseId);
                var module = modules.OrderByDescending(m => m.ModuleId).First();
                moduleViewModel.ModuleId = module.ModuleId;
                moduleViewModel.RoadmapFilePath = await fileStorageService.UploadAsync(file, module.ModuleId, "modules");
                await moduleService.PostEditAsync(moduleViewModel);
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

        [HttpGet, ActionName("Content")]
        public async Task<IActionResult> ContentPartialAsync(int moduleId)
        {
            var module = await moduleRepository.GetModuleByIdAsync(moduleId);
            if (module is null) return NotFound();
            return PartialView("_Content", module);
        }

        [HttpGet, ActionName("Download")]
        public async Task<IActionResult> DownloadFileAsync(int moduleId)
        {
            var module = await moduleRepository.GetModuleByIdAsync(moduleId);
            if (module is null || string.IsNullOrEmpty(module.RoadmapFilePath))
                return NotFound();

            var (stream, contentType, fileName) = await fileStorageService.DownloadAsync(module.RoadmapFilePath);
            return File(stream, contentType, fileName);
        }

        [HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditModuleAsync(ModuleViewModel moduleViewModel, IFormFile? file)
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

            if (file is not null && file.Length > 0)
            {
                if (!string.IsNullOrEmpty(moduleViewModel.RoadmapFilePath))
                    await fileStorageService.DeleteAsync(moduleViewModel.RoadmapFilePath);

                moduleViewModel.RoadmapFilePath = await fileStorageService.UploadAsync(file, moduleViewModel.ModuleId);
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
