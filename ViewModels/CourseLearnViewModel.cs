namespace quiz_project.ViewModels
{
    public class CourseLearnViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public List<ModuleLearnItem> Modules { get; set; } = new();
    }

    public class ModuleLearnItem
    {
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public List<ChapterLearnItem> Chapters { get; set; } = new();
    }

    public class ChapterLearnItem
    {
        public int ChapterId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public bool IsCompleted { get; set; }
    }
}
