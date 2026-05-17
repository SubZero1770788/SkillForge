namespace quiz_project.ViewModels
{
    public class CourseStatisticsViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int TotalUsers { get; set; }
        public List<ModuleStatsViewModel> Modules { get; set; } = new();
    }

    public class ModuleStatsViewModel
    {
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int? QuizId { get; set; }
        public string? QuizTitle { get; set; }
        public int CompletedByUsers { get; set; }
        public int TotalUsers { get; set; }
        public List<ChapterStatsViewModel> Chapters { get; set; } = new();
    }

    public class ChapterStatsViewModel
    {
        public int ChapterId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public int? QuizId { get; set; }
        public string? QuizTitle { get; set; }
        public int CompletedByUsers { get; set; }
        public int TotalUsers { get; set; }
    }
}
