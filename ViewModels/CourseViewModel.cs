using System.ComponentModel.DataAnnotations;

namespace quiz_project.ViewModels
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsSequential { get; set; }
        public bool IsPaid { get; set; }
        public List<ModuleViewModel> Modules { get; set; } = new();
        public int PendingEnrollmentsCount { get; set; }
    }
}
