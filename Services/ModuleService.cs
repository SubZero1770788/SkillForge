using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class ModuleService(IModuleRepository moduleRepository, IModuleMapper moduleMapper,
        IFileStorageService fileStorageService, IQuizRepository quizRepository) : IModuleService
    {
        private async Task DeleteQuizImagesAsync(int? quizId)
        {
            if (!quizId.HasValue) return;
            var quiz = await quizRepository.GetQuizByIdAsync(quizId.Value);
            if (quiz is null) return;
            var tasks = quiz.Questions
                .Where(q => !string.IsNullOrEmpty(q.ImagePath))
                .Select(q => fileStorageService.DeleteAsync(q.ImagePath!));
            await Task.WhenAll(tasks);
        }
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
            var module = await moduleRepository.GetModuleByIdAsync(moduleId); // includes Chapters
            if (module is null) return;

            // Delete chapter file attachments and their quiz images
            foreach (var chapter in module.Chapters)
            {
                if (!string.IsNullOrEmpty(chapter.FilePath))
                    await fileStorageService.DeleteAsync(chapter.FilePath);
                await DeleteQuizImagesAsync(chapter.QuizId);
            }

            // Delete module roadmap file
            if (!string.IsNullOrEmpty(module.RoadmapFilePath))
                await fileStorageService.DeleteAsync(module.RoadmapFilePath);

            // Delete module quiz images
            await DeleteQuizImagesAsync(module.QuizId);

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
