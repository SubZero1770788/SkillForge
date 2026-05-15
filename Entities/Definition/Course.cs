namespace quiz_project.Entities
{
    public class Course
    {
        public int CourseId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int TotalScore { get; set; }
        public bool IsPublic { get; set; }
        public List<Module> Modules { get; set; } = new();
        public User User { get; set; }
        public required int UserId { get; set; }
    }
}