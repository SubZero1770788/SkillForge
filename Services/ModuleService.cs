using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class ModuleService(IModuleRepository moduleRepository, IQuizRepository quizRepository,
        IModuleMapper moduleMapper) : IModuleService
    {
        public async Task<(bool success, string error)> CreateAsync(ModuleViewModel moduleViewModel)
        {
            var module = moduleMapper.ToEntity(moduleViewModel);
            await moduleRepository.CreateModuleAsync(module);

            if (module.QuizId.HasValue)
                await quizRepository.UpdateQuizScopeAsync(module.QuizId.Value, QuizScope.Module);

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

            // Update scope on old quiz if quiz changed
            if (existing.QuizId.HasValue && existing.QuizId != moduleViewModel.QuizId)
                await quizRepository.UpdateQuizScopeAsync(existing.QuizId.Value, QuizScope.Standalone);

            if (moduleViewModel.QuizId.HasValue)
                await quizRepository.UpdateQuizScopeAsync(moduleViewModel.QuizId.Value, QuizScope.Module);

            var module = moduleMapper.ToEntity(moduleViewModel);
            await moduleRepository.UpdateModuleAsync(module);
            return (true, Enumerable.Empty<string>());
        }

        public async Task DeleteAsync(int moduleId)
        {
            var module = await moduleRepository.GetModuleByIdAsync(moduleId);
            if (module is null) return;

            if (module.QuizId.HasValue)
                await quizRepository.UpdateQuizScopeAsync(module.QuizId.Value, QuizScope.Standalone);

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
