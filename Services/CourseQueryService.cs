using Microsoft.AspNetCore.Identity;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class CourseQueryService(ICourseRepository courseRepository, ICourseMapper courseMapper,
        IModuleRepository moduleRepository, IChapterRepository chapterRepository,
        IProgressRepository progressRepository, IQuizRepository quizRepository,
        UserManager<User> userManager) : ICourseQueryService
    {
        public async Task<bool> CheckIfPublicAsync(int Id)
        {
            var course = await courseRepository.GetCourseByIdAsync(Id);
            return course.IsPublic;
        }

        public async Task<List<CourseViewModel>> GetUserCoursesAsync(int userId)
        {
            var courses = await courseRepository.GetCoursesByUserAsync(userId);
            return courses.Select(c => courseMapper.ToCourseViewModel(c)).ToList();
        }

        public async Task<List<CourseViewModel>> GetUserSignedUpCoursesAsync(int userId)
        {
            var courses = await courseRepository.GetCoursesByUserAsync(userId);
            return courses.Select(c => courseMapper.ToCourseViewModel(c)).ToList();
        }

        public async Task<List<CourseViewModel>> GetPublicCoursesAsync()
        {
            var courses = await courseRepository.GetPublicCourses();
            return courses.Select(c => courseMapper.ToCourseViewModel(c)).ToList();
        }

        public async Task<CourseStatisticsViewModel?> GetCourseStatisticsAsync(int courseId)
        {
            var course = await courseRepository.GetCourseByIdAsync(courseId);
            if (course is null) return null;

            var modules = (await moduleRepository.GetModulesByCourseIdAsync(courseId))
                .OrderBy(m => m.Order)
                .ToList();

            var allQuizIds = modules
                .Select(m => m.QuizId)
                .Concat(modules.SelectMany(m => m.Chapters.Select(c => c.QuizId)))
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var allQuizzes = await quizRepository.GetQuizesAsync();
            var quizTitles = allQuizzes
                .Where(q => allQuizIds.Contains(q.QuizId))
                .ToDictionary(q => q.QuizId, q => q.Title);

            var chapterProgresses = (await progressRepository.GetAllChapterProgressesForCourseAsync(courseId)).ToList();
            var moduleProgresses = (await progressRepository.GetAllModuleProgressesForCourseAsync(courseId)).ToList();

            var totalUsers = chapterProgresses.Select(p => p.UserId).Distinct().Count();

            var moduleStats = modules.Select(module =>
            {
                var chapters = module.Chapters.OrderBy(c => c.Order).ToList();

                var moduleCompletedUsers = moduleProgresses
                    .Where(p => p.ModuleId == module.ModuleId && p.IsCompleted)
                    .Select(p => p.UserId)
                    .Distinct()
                    .Count();

                var chapterStats = chapters.Select(chapter => new ChapterStatsViewModel
                {
                    ChapterId = chapter.ChapterId,
                    Title = chapter.Title,
                    Order = chapter.Order,
                    QuizId = chapter.QuizId,
                    QuizTitle = chapter.QuizId.HasValue ? quizTitles.GetValueOrDefault(chapter.QuizId.Value) : null,
                    CompletedByUsers = chapterProgresses
                        .Where(p => p.ChapterId == chapter.ChapterId && p.IsCompleted)
                        .Select(p => p.UserId)
                        .Distinct()
                        .Count(),
                    TotalUsers = totalUsers
                }).ToList();

                return new ModuleStatsViewModel
                {
                    ModuleId = module.ModuleId,
                    Title = module.Title,
                    Order = module.Order,
                    QuizId = module.QuizId,
                    QuizTitle = module.QuizId.HasValue ? quizTitles.GetValueOrDefault(module.QuizId.Value) : null,
                    CompletedByUsers = moduleCompletedUsers,
                    TotalUsers = totalUsers,
                    Chapters = chapterStats
                };
            }).ToList();

            return new CourseStatisticsViewModel
            {
                CourseId = courseId,
                Title = course.Title,
                TotalUsers = totalUsers,
                Modules = moduleStats
            };
        }
    }
}
