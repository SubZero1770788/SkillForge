using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IChapterService
    {
        Task<ChapterViewModel?> GetViewAsync(int chapterId, int userId);
        Task<ChapterViewModel?> GetEditAsync(int chapterId);
        Task<(bool success, string error)> CreateAsync(ChapterViewModel chapterViewModel);
        Task<(bool success, IEnumerable<string> errors)> PostEditAsync(ChapterViewModel chapterViewModel);
        Task DeleteAsync(int chapterId);
        Task MarkAsCompletedAsync(int chapterId, int userId);
        Task TryCompleteModuleByQuizAsync(int quizId, int userId, int score, int totalScore);
        Task ReorderAsync(List<ReorderItem> items);
    }
}
