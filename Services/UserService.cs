using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quiz_project.Common;
using quiz_project.Entities;
using quiz_project.Extensions;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using System;

namespace quiz_project.Services
{
    public class UserService(UserManager<User> userManager, RoleManager<Role> roleManager,
                    IUserMapper userMapper, SignInManager<User> signInManager, IUserRepository userRepository,
                    ICreatorApplicationRepository creatorApplicationRepository) : IUserService
    {
        public async Task<List<ActiveUserViewModel>> GetAllActiveUsersAsync()
        {
            var users = await userManager.Users.Where(u => u.IsLoggedIn == true).ToListAsync();
            var scores = await userRepository.GetTotalScoresAsync();
            List<ActiveUserViewModel> activeUserViewModels = [];
            users.ForEach(u =>
            {
                var userScore = scores.GetValueOrDefault(u.Id, 0);
                var activeUsers = userMapper.ToActiveUserViewModel(u, userScore);
                activeUserViewModels.Add(activeUsers);
            });

            return activeUserViewModels;
        }

        public async Task<(bool, string error)> LoginAsync(LoginViewModel loginViewModel)
        {
            var user = await userManager.FindByNameAsync(loginViewModel.UserName);
            string error;
            if (user is null)
            {
                error = ErrorMessagesExtension.GetMessage(ErrorMessages.InvalidLoginAttempt);
                return (false, error);
            }

            var res = await signInManager.PasswordSignInAsync(loginViewModel.UserName, loginViewModel.Password, isPersistent: false, lockoutOnFailure: true);

            if (res.IsLockedOut)
            {
                error = ErrorMessagesExtension.GetMessage(ErrorMessages.AccountLocked);
                return (false, error);
            }

            else if (res.Succeeded)
            {
                return (true, string.Empty);
            }

            await userManager.AccessFailedAsync(user);

            var accessfailed = await userManager.GetAccessFailedCountAsync(user);
            var maxFailedAttempts = userManager.Options.Lockout.MaxFailedAccessAttempts;
            int leftAttempts = maxFailedAttempts - accessfailed;

            error = $"Invalid login attempt. {leftAttempts} left before account lockout.";
            return (false, error);
        }

        public async Task<bool> LogoutAsync(ClaimsPrincipal principal)
        {
            var user = principal;
            if (user is not null)
            {
                var userEntity = await userManager.FindByNameAsync(user.Identity.Name);
                await signInManager.SignOutAsync();
                return true;
            }
            return false;
        }

        public async Task<(bool, string error)> RegisterAsync(RegisterViewModel registerViewModel)
        {
            var userNameTaken = await userManager.FindByNameAsync(registerViewModel.UserName);
            if (userNameTaken is not null) return (false, "The username is already taken");

            var user = userMapper.ToEntity(registerViewModel);

            var created = await userManager.CreateAsync(user, registerViewModel.Password);
            if (created.Succeeded)
            {
                var defaultRole = await roleManager.FindByNameAsync("User");

                await userManager.AddToRoleAsync(user, defaultRole!.Name!);
                await userManager.UpdateAsync(user);
                await signInManager.SignInAsync(user, isPersistent: false);
                return (true, String.Empty);

            }

            return (false, string.Join(" ", created.Errors.Select(e => e.Description)));

        }

        public async Task<(bool, string error)> RegisterCreatorAsync(RegisterCreatorViewModel vm)
        {
            if (await userManager.FindByNameAsync(vm.UserName) is not null)
                return (false, "The username is already taken.");

            if (await userManager.FindByEmailAsync(vm.Email) is not null)
                return (false, "An account with this email already exists.");

            var user = new User { UserName = vm.UserName, Email = vm.Email };
            var created = await userManager.CreateAsync(user, vm.Password);
            if (!created.Succeeded)
                return (false, string.Join("; ", created.Errors.Select(e => e.Description)));

            var userRole = await roleManager.FindByNameAsync("User");
            await userManager.AddToRoleAsync(user, userRole!.Name!);

            await creatorApplicationRepository.AddAsync(new CreatorApplication
            {
                UserId = user.Id,
                ContactName = vm.ContactName,
                Description = vm.Description,
                AppliedAt = DateTime.UtcNow
            });

            return (true, string.Empty);
        }

        public async Task<(bool, string error)> ChangeEmailAsync(string username, EmailChangeViewModel emailChangeViewModel)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return (false, "User not found.");

            var passwordValid = await userManager.CheckPasswordAsync(user, emailChangeViewModel.Password);
            if (!passwordValid)
                return (false, "Invalid password.");

            if (string.IsNullOrWhiteSpace(emailChangeViewModel.newEmail))
                return (false, "New email cannot be empty.");

            if (!string.Equals(emailChangeViewModel.newEmail, emailChangeViewModel.confirmedNewEmail, StringComparison.OrdinalIgnoreCase))
                return (false, "New email addresses do not match.");

            if (string.Equals(user.Email, emailChangeViewModel.newEmail, StringComparison.OrdinalIgnoreCase))
                return (false, "New email must be different from the current email.");

            var existing = await userManager.FindByEmailAsync(emailChangeViewModel.newEmail);
            if (existing != null)
                return (false, "This email is already in use.");

            var result = await userManager.SetEmailAsync(user, emailChangeViewModel.newEmail);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return (true, string.Empty);
        }

        public async Task<(bool, string error)> ChangePasswordAsync(string username, PasswordChangeViewModel passwordChangeViewModel)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
                return (false, "User not found.");

            var result = await userManager.ChangePasswordAsync(user, passwordChangeViewModel.oldPassword, passwordChangeViewModel.newPassword);

            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return (false, string.Join("; ", updateResult.Errors.Select(e => e.Description)));

            return (true, string.Empty);
        }
    }
}