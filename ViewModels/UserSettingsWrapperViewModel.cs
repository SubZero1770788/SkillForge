using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class UserSettingsWrapperViewModel
    {

            public PasswordChangeViewModel PasswordChangeViewModel { get; set; }
            public EmailChangeViewModel EmailChangeViewModel { get; set; }
    }
}