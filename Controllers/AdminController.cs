using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(
        UserManager<User> userManager,
        ICreatorApplicationRepository applicationRepository) : Controller
    {
        [HttpGet, ActionName("Users")]
        public async Task<IActionResult> UsersAsync()
        {
            var users = userManager.Users.ToList();
            var items = new List<AdminUserItem>();

            foreach (var u in users)
            {
                var roles = (await userManager.GetRolesAsync(u)).ToList();
                var lockout = await userManager.IsLockedOutAsync(u);
                items.Add(new AdminUserItem
                {
                    UserId = u.Id,
                    UserName = u.UserName ?? "-",
                    Email = u.Email ?? "-",
                    Roles = roles,
                    IsLockedOut = lockout
                });
            }

            var pendingCount = (await applicationRepository.GetByStatusAsync(ApplicationStatus.Pending)).Count;

            var vm = new AdminUserListViewModel
            {
                Users = items.OrderBy(u => u.UserName).ToList(),
                PendingApplicationsCount = pendingCount
            };
            return View(vm);
        }

        [HttpPost, ActionName("SetRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRoleAsync(int userId, string role, bool grant)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return NotFound();

            if (User.Identity?.Name == user.UserName && role == "Admin" && !grant)
            {
                TempData["Error"] = "You cannot remove your own Admin role.";
                return RedirectToAction("Users");
            }

            if (grant)
                await userManager.AddToRoleAsync(user, role);
            else
                await userManager.RemoveFromRoleAsync(user, role);

            return RedirectToAction("Users");
        }

        [HttpPost, ActionName("LockUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockUserAsync(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return NotFound();

            if (User.Identity?.Name == user.UserName)
            {
                TempData["Error"] = "You cannot lock your own account.";
                return RedirectToAction("Users");
            }

            await userManager.SetLockoutEnabledAsync(user, true);
            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return RedirectToAction("Users");
        }

        [HttpPost, ActionName("UnlockUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockUserAsync(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null) return NotFound();

            await userManager.SetLockoutEndDateAsync(user, null);
            await userManager.ResetAccessFailedCountAsync(user);
            return RedirectToAction("Users");
        }

        [HttpGet, ActionName("Applications")]
        public async Task<IActionResult> ApplicationsAsync()
        {
            var all = await applicationRepository.GetAllAsync();
            var vm = new AdminApplicationsViewModel
            {
                Pending = all.Where(a => a.Status == ApplicationStatus.Pending)
                             .Select(ToItem).ToList(),
                Reviewed = all.Where(a => a.Status != ApplicationStatus.Pending)
                              .Select(ToItem).ToList()
            };
            return View(vm);
        }

        [HttpPost, ActionName("ApproveApplication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveApplicationAsync(int applicationId)
        {
            var app = await applicationRepository.GetByIdAsync(applicationId);
            if (app is null) return NotFound();

            // Grant Creator role
            var user = await userManager.FindByIdAsync(app.UserId.ToString());
            if (user is not null && !await userManager.IsInRoleAsync(user, "Creator"))
                await userManager.AddToRoleAsync(user, "Creator");

            app.Status = ApplicationStatus.Approved;
            app.ReviewedAt = DateTime.UtcNow;
            await applicationRepository.UpdateAsync(app);

            TempData["Success"] = $"{app.User?.UserName ?? "User"} has been granted Creator access.";
            return RedirectToAction("Applications");
        }

        [HttpPost, ActionName("RejectApplication")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectApplicationAsync(ReviewApplicationViewModel model)
        {
            var app = await applicationRepository.GetByIdAsync(model.ApplicationId);
            if (app is null) return NotFound();

            app.Status = ApplicationStatus.Rejected;
            app.ReviewedAt = DateTime.UtcNow;
            app.ReviewNote = model.ReviewNote;
            await applicationRepository.UpdateAsync(app);

            TempData["Success"] = "Application rejected.";
            return RedirectToAction("Applications");
        }

        private static CreatorApplicationItem ToItem(CreatorApplication a) => new()
        {
            ApplicationId = a.Id,
            UserId = a.UserId,
            UserName = a.User?.UserName ?? "-",
            Email = a.User?.Email ?? "-",
            ContactName = a.ContactName,
            Description = a.Description,
            Status = a.Status,
            AppliedAt = a.AppliedAt,
            ReviewedAt = a.ReviewedAt,
            ReviewNote = a.ReviewNote
        };
    }
}
