using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IChapterMapper
    {
        Chapter ToEntity(ChapterViewModel viewModel);
        ChapterViewModel ToViewModel(Chapter chapter, bool isCompleted = false, bool isLocked = false);
    }
}
