using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class ChapterService(IChapterRepository chapterRepository, IModuleRepository moduleRepository,
        ICourseRepository courseRepository, IProgressRepository progressRepository,
        IChapterMapper chapterMapper, IAttemptRepository attemptRepository,
        IQuizRepository quizRepository, IFileStorageService fileStorageService) : IChapterService
    {
        private static bool IsQuizPassed(QuizAttempt? best, Quiz? quiz)
        {
            if (best is null || quiz is null) return false;
            if (quiz.PassPercentage == 0) return true;
            if (quiz.TotalScore == 0) return false;
            return (double)best.Score / quiz.TotalScore * 100 >= quiz.PassPercentage;
        }

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
                else
                {
                    var allModules = (await moduleRepository.GetModulesByCourseIdAsync(module.CourseId))
                        .OrderBy(m => m.Order)
                        .ToList();
                    var moduleIndex = allModules.FindIndex(m => m.ModuleId == module.ModuleId);
                    if (moduleIndex > 0)
                    {
                        var previousModule = allModules[moduleIndex - 1];
                        var prevModProgress = await progressRepository.GetModuleProgressAsync(userId, previousModule.ModuleId);
                        isLocked = prevModProgress?.IsCompleted != true;
                    }
                }
            }

            bool quizPassed = false;
            if (chapter.QuizId.HasValue)
            {
                var quiz = await quizRepository.GetQuizByIdAsync(chapter.QuizId.Value);
                var best = await attemptRepository.GetTopUserAttemptAsync(userId, chapter.QuizId.Value);
                quizPassed = IsQuizPassed(best, quiz);
            }

            var vm = chapterMapper.ToViewModel(chapter, isCompleted, isLocked);
            vm.QuizPassed = quizPassed;
            return vm;
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
            return (true, string.Empty);
        }

        public async Task<(bool success, IEnumerable<string> errors)> PostEditAsync(ChapterViewModel chapterViewModel)
        {
            var existing = await chapterRepository.GetChapterByIdAsync(chapterViewModel.ChapterId);
            if (existing is null) return (false, new[] { "Chapter not found." });

            var chapter = chapterMapper.ToEntity(chapterViewModel);
            await chapterRepository.UpdateChapterAsync(chapter);
            return (true, Enumerable.Empty<string>());
        }

        public async Task DeleteAsync(int chapterId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return;

            if (!string.IsNullOrEmpty(chapter.FilePath))
                await fileStorageService.DeleteAsync(chapter.FilePath);

            if (chapter.QuizId.HasValue)
            {
                var quiz = await quizRepository.GetQuizByIdAsync(chapter.QuizId.Value);
                if (quiz is not null)
                {
                    var imageTasks = quiz.Questions
                        .Where(q => !string.IsNullOrEmpty(q.ImagePath))
                        .Select(q => fileStorageService.DeleteAsync(q.ImagePath!));
                    await Task.WhenAll(imageTasks);
                }
            }

            await chapterRepository.DeleteChapterAsync(chapter);
        }

        public async Task MarkAsCompletedAsync(int chapterId, int userId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return;

            var module = await moduleRepository.GetModuleByIdAsync(chapter.ModuleId);
            if (module is null) return;

            var course = await courseRepository.GetCourseByIdAsync(module.CourseId);
            if (course is null) return;

            if (course.IsSequential)
            {
                var chapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId))
                    .OrderBy(c => c.Order)
                    .ToList();

                var index = chapters.FindIndex(c => c.ChapterId == chapterId);
                if (index > 0)
                {
                    var prevProgress = await progressRepository.GetChapterProgressAsync(userId, chapters[index - 1].ChapterId);
                    if (prevProgress?.IsCompleted != true) return;
                }
                else
                {
                    var allModules = (await moduleRepository.GetModulesByCourseIdAsync(module.CourseId))
                        .OrderBy(m => m.Order)
                        .ToList();
                    var moduleIndex = allModules.FindIndex(m => m.ModuleId == module.ModuleId);
                    if (moduleIndex > 0)
                    {
                        var prevModProgress = await progressRepository.GetModuleProgressAsync(userId, allModules[moduleIndex - 1].ModuleId);
                        if (prevModProgress?.IsCompleted != true) return;
                    }
                }
            }

            if (chapter.QuizId.HasValue) return;

            await progressRepository.MarkChapterCompletedAsync(userId, chapterId);

            var moduleChapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId)).ToList();
            var progresses = await progressRepository.GetChapterProgressesForModuleAsync(userId, chapter.ModuleId);
            var completedIds = progresses.Where(p => p.IsCompleted).Select(p => p.ChapterId).ToHashSet();

            if (moduleChapters.All(c => completedIds.Contains(c.ChapterId)))
            {
                if (module.RequireQuizPass && module.QuizId.HasValue)
                    return;

                await progressRepository.MarkModuleCompletedAsync(userId, chapter.ModuleId);
            }
        }

        public async Task TryCompleteModuleByQuizAsync(int quizId, int userId, int score, int totalScore)
        {
            var module = await moduleRepository.GetModuleByQuizIdAsync(quizId);
            if (module is null || !module.RequireQuizPass) return;

            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            if (quiz is null) return;

            var passed = quiz.PassPercentage == 0
                || (totalScore > 0 && (double)score / totalScore * 100 >= quiz.PassPercentage);
            if (!passed) return;

            var chapters = (await chapterRepository.GetChaptersByModuleIdAsync(module.ModuleId)).ToList();
            if (!chapters.Any()) return;
            var progresses = await progressRepository.GetChapterProgressesForModuleAsync(userId, module.ModuleId);
            var completedIds = progresses.Where(p => p.IsCompleted).Select(p => p.ChapterId).ToHashSet();
            if (!chapters.All(c => completedIds.Contains(c.ChapterId))) return;

            await progressRepository.MarkModuleCompletedAsync(userId, module.ModuleId);
        }

        public async Task TryCompleteChapterByQuizAsync(int quizId, int userId, int score, int totalScore)
        {
            var chapter = await chapterRepository.GetChapterByQuizIdAsync(quizId);
            if (chapter is null) return;

            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            if (quiz is null) return;

            var passed = quiz.PassPercentage == 0
                || (totalScore > 0 && (double)score / totalScore * 100 >= quiz.PassPercentage);
            if (!passed) return;

            var module = await moduleRepository.GetModuleByIdAsync(chapter.ModuleId);
            if (module is null) return;

            var course = await courseRepository.GetCourseByIdAsync(module.CourseId);
            if (course is null) return;

            if (course.IsSequential)
            {
                var orderedChapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId))
                    .OrderBy(c => c.Order).ToList();
                var index = orderedChapters.FindIndex(c => c.ChapterId == chapter.ChapterId);
                if (index > 0)
                {
                    var prevProgress = await progressRepository.GetChapterProgressAsync(userId, orderedChapters[index - 1].ChapterId);
                    if (prevProgress?.IsCompleted != true) return;
                }
                else
                {
                    var allModules = (await moduleRepository.GetModulesByCourseIdAsync(module.CourseId))
                        .OrderBy(m => m.Order).ToList();
                    var moduleIndex = allModules.FindIndex(m => m.ModuleId == module.ModuleId);
                    if (moduleIndex > 0)
                    {
                        var prevModProgress = await progressRepository.GetModuleProgressAsync(userId, allModules[moduleIndex - 1].ModuleId);
                        if (prevModProgress?.IsCompleted != true) return;
                    }
                }
            }

            await progressRepository.MarkChapterCompletedAsync(userId, chapter.ChapterId);

            var moduleChapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId)).ToList();
            var progresses = await progressRepository.GetChapterProgressesForModuleAsync(userId, chapter.ModuleId);
            var completedIds = progresses.Where(p => p.IsCompleted).Select(p => p.ChapterId).ToHashSet();
            completedIds.Add(chapter.ChapterId); 

            if (moduleChapters.All(c => completedIds.Contains(c.ChapterId)))
            {
                if (!module.RequireQuizPass || !module.QuizId.HasValue)
                    await progressRepository.MarkModuleCompletedAsync(userId, chapter.ModuleId);
            }
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
