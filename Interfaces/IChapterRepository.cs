using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IChapterRepository
    {
        Task<Chapter?> GetChapterByIdAsync(int chapterId);
        Task<IEnumerable<Chapter>> GetChaptersByModuleIdAsync(int moduleId);
        Task CreateChapterAsync(Chapter chapter);
        Task UpdateChapterAsync(Chapter chapter);
        Task DeleteChapterAsync(Chapter chapter);
        Task<bool> UserHasEnrolledCourseWithQuizAsync(int userId, int quizId);
        Task<Chapter?> GetChapterByQuizIdAsync(int quizId);
        Task<Chapter?> GetChapterByQuizIdForUserAsync(int quizId, int userId);
    }
}
