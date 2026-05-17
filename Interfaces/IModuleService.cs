using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IModuleService
    {
        Task<(bool success, string error)> CreateAsync(ModuleViewModel moduleViewModel);
        Task<ModuleViewModel?> GetEditAsync(int moduleId);
        Task<(bool success, IEnumerable<string> errors)> PostEditAsync(ModuleViewModel moduleViewModel);
        Task DeleteAsync(int moduleId);
        Task ReorderAsync(List<ReorderItem> items);
    }
}
