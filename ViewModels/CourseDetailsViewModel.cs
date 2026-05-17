using quiz_project.Entities;

namespace quiz_project.ViewModels
{
    public class CourseDetailsViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreatorName { get; set; }
        public bool IsPaid { get; set; }
        public bool IsSequential { get; set; }
        public int ModuleCount { get; set; }
        public int ChapterCount { get; set; }
        public List<ModuleDetailsViewModel> Modules { get; set; } = new();
        public EnrollmentStatus? UserEnrollmentStatus { get; set; }
    }

    public class ModuleDetailsViewModel
    {
        public string Title { get; set; }
        public int ChapterCount { get; set; }
        public bool HasQuiz { get; set; }
    }
}
