namespace quiz_project.Entities
{
    public class Chapter
    {
        public int ChapterId { get; set; }
        public required string Title { get; set; }
        public int Order { get; set; }
        public Module Module { get; set; }
        public required int ModuleId { get; set; }
        public string? Content { get; set; }
        public string? FilePath { get; set; }
        public Quiz? Quiz { get; set; }
        public int? QuizId { get; set; }
    }
}
