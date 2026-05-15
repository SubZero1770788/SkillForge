using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class EmailChangeViewModel
    {
        public string Password { get; set; }
        public string oldEmail { get; set; }
        public string newEmail { get; set; }
        public string confirmedNewEmail { get; set; }
    }
}