using System.ComponentModel.DataAnnotations;

namespace quiz_project.ViewModels
{
    public class ChapterViewModel
    {
        public int ChapterId { get; set; }
        public int ModuleId { get; set; }
        [Required]
        public string Title { get; set; }
        public int Order { get; set; }
        public string? Content { get; set; }
        public string? FilePath { get; set; }
        public int? QuizId { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsLocked { get; set; }
        public bool RequireQuizPass { get; set; }
    }
}
