namespace quiz_project.Entities
{
    public class Module
    {
        public int ModuleId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int Order { get; set; }
        public string? RoadmapFilePath { get; set; }
        public int? QuizId { get; set; }
        public bool RequireQuizPass { get; set; } = false;
        public List<Chapter> Chapters { get; set; } = new();
        public Course Course { get; set; }
        public required int CourseId { get; set; }
    }
}
