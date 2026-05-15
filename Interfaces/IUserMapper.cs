using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.ViewModels;
using quiz_project.ViewModels.Mappers;

namespace quiz_project.Interfaces
{
    public interface IUserMapper
    {
        public User ToEntity(RegisterViewModel registerViewModel);
        public ActiveUserViewModel ToActiveUserViewModel(User user, int TotalScore);
    }
}