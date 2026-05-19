namespace quiz_project.ViewModels
{
    public class ModuleContentViewModel
    {
        public int ModuleId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? RoadmapFilePath { get; set; }
        public int? QuizId { get; set; }
        public bool RequireQuizPass { get; set; }
        public bool QuizPassed { get; set; }
    }
}
