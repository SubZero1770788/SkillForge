namespace quiz_project.ViewModels
{
    public class CourseDashboardViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int TotalChapters { get; set; }
        public int CompletedChapters { get; set; }
        public double CompletionPercent => TotalChapters == 0 ? 0 : Math.Round((double)CompletedChapters / TotalChapters * 100, 1);
        public int? ResumeChapterId { get; set; }
        public string? ResumeChapterTitle { get; set; }
        public List<CourseQuizResult> QuizResults { get; set; } = new();
    }

    public class CourseQuizResult
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public string Context { get; set; }
        public int BestScore { get; set; }
        public int TotalScore { get; set; }
        public int PassPercentage { get; set; }
        public double BestPercent => TotalScore == 0 ? 0 : Math.Round((double)BestScore / TotalScore * 100, 1);
        public bool Passed => PassPercentage == 0 || BestPercent >= PassPercentage;
        public bool Attempted { get; set; }
    }
}
