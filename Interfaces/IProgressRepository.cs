using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IProgressRepository
    {
        Task<UserModuleProgress?> GetModuleProgressAsync(int userId, int moduleId);
        Task<UserPartProgress?> GetChapterProgressAsync(int userId, int chapterId);
        Task<IEnumerable<UserPartProgress>> GetChapterProgressesForModuleAsync(int userId, int moduleId);
        Task MarkChapterCompletedAsync(int userId, int chapterId);
        Task MarkModuleCompletedAsync(int userId, int moduleId);
    }
}
