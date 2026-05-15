using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.ViewModels.Mappers
{
    public class UserMapper : IUserMapper
    {
        public User ToEntity(RegisterViewModel registerViewModel)
        {
            var user = new User()
            {
                UserName = registerViewModel.UserName,
                Email = registerViewModel.Email
            };

            return user;
        }

        public ActiveUserViewModel ToActiveUserViewModel(User user, int TotalScore)
        {
            var activeUserViewModel = new ActiveUserViewModel()
            {
                UserName = user.UserName,
                TotalScore = TotalScore
            };

            return activeUserViewModel;
        }
    }
}