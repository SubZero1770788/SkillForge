using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.ViewModels.Mappers
{
    public class ChapterMapper : IChapterMapper
    {
        public Chapter ToEntity(ChapterViewModel viewModel)
        {
            return new Chapter
            {
                ChapterId = viewModel.ChapterId,
                ModuleId = viewModel.ModuleId,
                Title = viewModel.Title,
                Order = viewModel.Order,
                Content = viewModel.Content,
                FilePath = viewModel.FilePath,
                QuizId = viewModel.QuizId
            };
        }

        public ChapterViewModel ToViewModel(Chapter chapter, bool isCompleted = false, bool isLocked = false)
        {
            return new ChapterViewModel
            {
                ChapterId = chapter.ChapterId,
                ModuleId = chapter.ModuleId,
                Title = chapter.Title,
                Order = chapter.Order,
                Content = chapter.Content,
                FilePath = chapter.FilePath,
                QuizId = chapter.QuizId,
                IsCompleted = isCompleted,
                IsLocked = isLocked
            };
        }
    }
}
