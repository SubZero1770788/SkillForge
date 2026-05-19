using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IModuleRepository
    {
        Task<Module?> GetModuleByIdAsync(int moduleId);
        Task<Module?> GetModuleByQuizIdAsync(int quizId);
        Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId);
        Task CreateModuleAsync(Module module);
        Task UpdateModuleAsync(Module module);
        Task DeleteModuleAsync(Module module);
    }
}
