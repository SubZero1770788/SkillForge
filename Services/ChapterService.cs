using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class ChapterService(IChapterRepository chapterRepository, IModuleRepository moduleRepository,
        ICourseRepository courseRepository, IProgressRepository progressRepository,
        IQuizRepository quizRepository, IChapterMapper chapterMapper) : IChapterService
    {
        public async Task<ChapterViewModel?> GetViewAsync(int chapterId, int userId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return null;

            var module = await moduleRepository.GetModuleByIdAsync(chapter.ModuleId);
            if (module is null) return null;

            var course = await courseRepository.GetCourseByIdAsync(module.CourseId);
            if (course is null) return null;

            var isCompleted = (await progressRepository.GetChapterProgressAsync(userId, chapterId))?.IsCompleted ?? false;
            var isLocked = false;

            if (course.IsSequential)
            {
                var chapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId))
                    .OrderBy(c => c.Order)
                    .ToList();

                var index = chapters.FindIndex(c => c.ChapterId == chapterId);
                if (index > 0)
                {
                    var previousChapter = chapters[index - 1];
                    var previousProgress = await progressRepository.GetChapterProgressAsync(userId, previousChapter.ChapterId);
                    isLocked = previousProgress?.IsCompleted != true;
                }
            }

            return chapterMapper.ToViewModel(chapter, isCompleted, isLocked);
        }

        public async Task<ChapterViewModel?> GetEditAsync(int chapterId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return null;
            return chapterMapper.ToViewModel(chapter);
        }

        public async Task<(bool success, string error)> CreateAsync(ChapterViewModel chapterViewModel)
        {
            var chapter = chapterMapper.ToEntity(chapterViewModel);
            await chapterRepository.CreateChapterAsync(chapter);

            if (chapter.QuizId.HasValue)
                await quizRepository.UpdateQuizScopeAsync(chapter.QuizId.Value, QuizScope.Chapter);

            return (true, string.Empty);
        }

        public async Task<(bool success, IEnumerable<string> errors)> PostEditAsync(ChapterViewModel chapterViewModel)
        {
            var existing = await chapterRepository.GetChapterByIdAsync(chapterViewModel.ChapterId);
            if (existing is null) return (false, new[] { "Chapter not found." });

            if (existing.QuizId.HasValue && existing.QuizId != chapterViewModel.QuizId)
                await quizRepository.UpdateQuizScopeAsync(existing.QuizId.Value, QuizScope.Standalone);

            if (chapterViewModel.QuizId.HasValue)
                await quizRepository.UpdateQuizScopeAsync(chapterViewModel.QuizId.Value, QuizScope.Chapter);

            var chapter = chapterMapper.ToEntity(chapterViewModel);
            await chapterRepository.UpdateChapterAsync(chapter);
            return (true, Enumerable.Empty<string>());
        }

        public async Task DeleteAsync(int chapterId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return;

            if (chapter.QuizId.HasValue)
                await quizRepository.UpdateQuizScopeAsync(chapter.QuizId.Value, QuizScope.Standalone);

            await chapterRepository.DeleteChapterAsync(chapter);
        }

        public async Task MarkAsCompletedAsync(int chapterId, int userId)
        {
            await progressRepository.MarkChapterCompletedAsync(userId, chapterId);

            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return;

            var chapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId)).ToList();
            var progresses = await progressRepository.GetChapterProgressesForModuleAsync(userId, chapter.ModuleId);
            var completedIds = progresses.Where(p => p.IsCompleted).Select(p => p.ChapterId).ToHashSet();

            if (chapters.All(c => completedIds.Contains(c.ChapterId)))
                await progressRepository.MarkModuleCompletedAsync(userId, chapter.ModuleId);
        }

        public async Task ReorderAsync(List<ReorderItem> items)
        {
            foreach (var item in items)
            {
                var chapter = await chapterRepository.GetChapterByIdAsync(item.Id);
                if (chapter is null) continue;
                chapter.Order = item.Order;
                await chapterRepository.UpdateChapterAsync(chapter);
            }
        }
    }
}
