using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class PasswordChangeViewModel
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string confirmedNewPassword { get; set; }
    }
}