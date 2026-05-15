namespace quiz_project.Entities
{
    public class Chapter
    {
        public int ChapterId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public bool IsPublic { get; set; }
        public Course Course { get; set; }
        public required int CourseId { get; set; }
        public Module Module { get; set; }
        public required int ModuleId { get; set; }
        public Quiz? Quiz { get; set; }
        public int? QuizId { get; set; }
    }
}