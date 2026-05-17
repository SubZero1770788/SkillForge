using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class ModuleService(IModuleRepository moduleRepository, IModuleMapper moduleMapper) : IModuleService
    {
        public async Task<(bool success, string error)> CreateAsync(ModuleViewModel moduleViewModel)
        {
            var module = moduleMapper.ToEntity(moduleViewModel);
            await moduleRepository.CreateModuleAsync(module);
            return (true, string.Empty);
        }

        public async Task<ModuleViewModel?> GetEditAsync(int moduleId)
        {
            var module = await moduleRepository.GetModuleByIdAsync(moduleId);
            if (module is null) return null;
            return moduleMapper.ToViewModel(module);
        }

        public async Task<(bool success, IEnumerable<string> errors)> PostEditAsync(ModuleViewModel moduleViewModel)
        {
            var existing = await moduleRepository.GetModuleByIdAsync(moduleViewModel.ModuleId);
            if (existing is null) return (false, new[] { "Module not found." });

            var module = moduleMapper.ToEntity(moduleViewModel);
            await moduleRepository.UpdateModuleAsync(module);
            return (true, Enumerable.Empty<string>());
        }

        public async Task DeleteAsync(int moduleId)
        {
            var module = await moduleRepository.GetModuleByIdAsync(moduleId);
            if (module is null) return;
            await moduleRepository.DeleteModuleAsync(module);
        }

        public async Task ReorderAsync(List<ReorderItem> items)
        {
            foreach (var item in items)
            {
                var module = await moduleRepository.GetModuleByIdAsync(item.Id);
                if (module is null) continue;
                module.Order = item.Order;
                await moduleRepository.UpdateModuleAsync(module);
            }
        }
    }
}
