using System.ComponentModel.DataAnnotations;

namespace quiz_project.ViewModels
{
    public class ModuleViewModel
    {
        public int ModuleId { get; set; }
        public int CourseId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public int Order { get; set; }
        public string? RoadmapFilePath { get; set; }
        public int? QuizId { get; set; }
        public bool RequireQuizPass { get; set; }
        public List<ChapterViewModel> Chapters { get; set; } = new();
    }
}
