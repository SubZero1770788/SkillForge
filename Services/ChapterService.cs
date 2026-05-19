using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class ChapterService(IChapterRepository chapterRepository, IModuleRepository moduleRepository,
        ICourseRepository courseRepository, IProgressRepository progressRepository,
        IChapterMapper chapterMapper, IAttemptRepository attemptRepository,
        IQuizRepository quizRepository) : IChapterService
    {
        // Returns true if the user's best score for quizId satisfies the pass threshold.
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
            }

            // Check if the chapter quiz is passed (for the RequireQuizPass UI gate)
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
            await chapterRepository.DeleteChapterAsync(chapter);
        }

        public async Task MarkAsCompletedAsync(int chapterId, int userId)
        {
            var chapter = await chapterRepository.GetChapterByIdAsync(chapterId);
            if (chapter is null) return;

            // Gate: chapter requires passing its quiz
            if (chapter.RequireQuizPass && chapter.QuizId.HasValue)
            {
                var quiz = await quizRepository.GetQuizByIdAsync(chapter.QuizId.Value);
                var best = await attemptRepository.GetTopUserAttemptAsync(userId, chapter.QuizId.Value);
                if (!IsQuizPassed(best, quiz)) return;
            }

            await progressRepository.MarkChapterCompletedAsync(userId, chapterId);

            var chapters = (await chapterRepository.GetChaptersByModuleIdAsync(chapter.ModuleId)).ToList();
            var progresses = await progressRepository.GetChapterProgressesForModuleAsync(userId, chapter.ModuleId);
            var completedIds = progresses.Where(p => p.IsCompleted).Select(p => p.ChapterId).ToHashSet();

            if (chapters.All(c => completedIds.Contains(c.ChapterId)))
            {
                var module = await moduleRepository.GetModuleByIdAsync(chapter.ModuleId);
                if (module is null) return;

                // Gate: module requires passing its own quiz before being marked complete
                if (module.RequireQuizPass && module.QuizId.HasValue)
                    return; // Module quiz must be passed separately — handled by TryCompleteModuleByQuizAsync

                await progressRepository.MarkModuleCompletedAsync(userId, chapter.ModuleId);
            }
        }

        public async Task TryCompleteModuleByQuizAsync(int quizId, int userId, int score, int totalScore)
        {
            var module = await moduleRepository.GetModuleByQuizIdAsync(quizId);
            if (module is null || !module.RequireQuizPass) return;

            // Verify pass threshold using the quiz definition
            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            if (quiz is null) return;

            // Build a fake attempt wrapper just to reuse the helper
            var passed = quiz.PassPercentage == 0
                || (totalScore > 0 && (double)score / totalScore * 100 >= quiz.PassPercentage);
            if (!passed) return;

            // All chapters in the module must already be completed
            var chapters = (await chapterRepository.GetChaptersByModuleIdAsync(module.ModuleId)).ToList();
            if (!chapters.Any()) return;
            var progresses = await progressRepository.GetChapterProgressesForModuleAsync(userId, module.ModuleId);
            var completedIds = progresses.Where(p => p.IsCompleted).Select(p => p.ChapterId).ToHashSet();
            if (!chapters.All(c => completedIds.Contains(c.ChapterId))) return;

            await progressRepository.MarkModuleCompletedAsync(userId, module.ModuleId);
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
