using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{12,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, a digit, and a special character.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm your password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [Required]
        public string ConfirmedPassword { get; set; }
    }
}