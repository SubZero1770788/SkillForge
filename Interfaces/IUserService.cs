using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.ViewModels;
using quiz_project.ViewModels;


namespace quiz_project.Interfaces
{
    public interface IUserService
    {
        public Task<(bool, string error)> LoginAsync(LoginViewModel loginViewModel);
        public Task<(bool, string error)> RegisterAsync(RegisterViewModel registerViewModel);
        public Task<bool> LogoutAsync(ClaimsPrincipal principal);
        public Task<List<ActiveUserViewModel>> GetAllActiveUsersAsync();
        public Task<(bool, string error)> ChangePasswordAsync(string username, PasswordChangeViewModel passwordChangeViewModel);
        public Task<(bool, string error)> ChangeEmailAsync(string username, EmailChangeViewModel emailChangeViewModel);
    }
}