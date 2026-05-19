using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Controllers
{
    [Authorize(Roles = "User")]
    public class ReminderController(
        IQuizReminderService reminderService,
        UserManager<User> userManager) : Controller
    {
        [HttpGet, ActionName("MyQuizzes")]
        public async Task<IActionResult> MyQuizzesAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            var vm = await reminderService.GetMyQuizzesAsync(user.Id);
            return View(vm);
        }

        [HttpGet, ActionName("DueCount")]
        public async Task<IActionResult> DueCountAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return Json(new { count = 0 });

            var count = await reminderService.GetDueCountAsync(user.Id);
            return Json(new { count });
        }

        [HttpPost, ActionName("AddReminder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReminderAsync(int quizId, string? returnUrl)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            await reminderService.AddReminderAsync(user.Id, quizId);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("MyQuizzes");
        }

        [HttpPost, ActionName("RemoveReminder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveReminderAsync(int reminderId, string? returnUrl)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User");

            await reminderService.RemoveReminderAsync(reminderId, user.Id);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("MyQuizzes");
        }
    }
}
