using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.ViewModels.Mappers
{
    public class ModuleMapper : IModuleMapper
    {
        public Module ToEntity(ModuleViewModel viewModel)
        {
            return new Module
            {
                ModuleId = viewModel.ModuleId,
                CourseId = viewModel.CourseId,
                Title = viewModel.Title,
                Description = viewModel.Description,
                Order = viewModel.Order,
                RoadmapFilePath = viewModel.RoadmapFilePath,
                QuizId = viewModel.QuizId
            };
        }

        public ModuleViewModel ToViewModel(Module module)
        {
            return new ModuleViewModel
            {
                ModuleId = module.ModuleId,
                CourseId = module.CourseId,
                Title = module.Title,
                Description = module.Description,
                Order = module.Order,
                RoadmapFilePath = module.RoadmapFilePath,
                QuizId = module.QuizId,
                Chapters = module.Chapters.Select(c => new ChapterViewModel
                {
                    ChapterId = c.ChapterId,
                    ModuleId = c.ModuleId,
                    Title = c.Title,
                    Order = c.Order,
                    Content = c.Content,
                    FilePath = c.FilePath,
                    QuizId = c.QuizId
                }).ToList()
            };
        }
    }
}
