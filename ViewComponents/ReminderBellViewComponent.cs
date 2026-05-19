using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.ViewComponents
{
    public class ReminderBellViewComponent(
        IQuizReminderService reminderService,
        UserManager<User> userManager) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!UserClaimsPrincipal.IsInRole("User"))
                return Content(string.Empty);

            var user = await userManager.GetUserAsync(UserClaimsPrincipal);
            if (user is null) return Content(string.Empty);

            var count = await reminderService.GetDueCountAsync(user.Id);
            return View(count);
        }
    }
}
