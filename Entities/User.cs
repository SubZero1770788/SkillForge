using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace quiz_project.Entities
{
    public class User : IdentityUser<int>
    {
        public List<Quiz>? Quizzes { get; set; }
        public override string? UserName { get; set; }
        public bool IsLoggedIn { get; set; } = false;

        // Excluding properties from IdentityUser that I don't need

        [NotMapped]
        public override string? PhoneNumber { get; set; }
        [NotMapped]
        public override bool PhoneNumberConfirmed { get; set; }
        [NotMapped]
        public override bool TwoFactorEnabled { get; set; }
    }
}