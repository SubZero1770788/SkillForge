using System.ComponentModel.DataAnnotations;

namespace quiz_project.ViewModels
{
    public class RegisterCreatorViewModel
    {
        [Required]
        public string UserName { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmedPassword { get; set; } = "";

        [Required, MaxLength(120)]
        [Display(Name = "Your name or company name")]
        public string ContactName { get; set; } = "";

        [Required, MinLength(30), MaxLength(1000)]
        [Display(Name = "What do you plan to publish? (describe your content briefly)")]
        public string Description { get; set; } = "";
    }
}
