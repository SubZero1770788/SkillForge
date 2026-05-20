using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class ChapterRepository(QuizDb context) : IChapterRepository
    {
        public async Task<Chapter?> GetChapterByIdAsync(int chapterId)
        {
            return await context.Chapters
                .FirstOrDefaultAsync(c => c.ChapterId == chapterId);
        }

        public async Task<IEnumerable<Chapter>> GetChaptersByModuleIdAsync(int moduleId)
        {
            return await context.Chapters
                .Where(c => c.ModuleId == moduleId)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task CreateChapterAsync(Chapter chapter)
        {
            await context.Chapters.AddAsync(chapter);
            await context.SaveChangesAsync();
        }

        public async Task UpdateChapterAsync(Chapter chapter)
        {
            var existing = await context.Chapters.FirstAsync(c => c.ChapterId == chapter.ChapterId);
            context.Entry(existing).CurrentValues.SetValues(chapter);
            await context.SaveChangesAsync();
        }

        public async Task DeleteChapterAsync(Chapter chapter)
        {
            var existing = await context.Chapters.FirstAsync(c => c.ChapterId == chapter.ChapterId);
            context.Chapters.Remove(existing);
            await context.SaveChangesAsync();
        }

        public async Task<Chapter?> GetChapterByQuizIdAsync(int quizId)
        {
            return await context.Chapters
                .FirstOrDefaultAsync(c => c.QuizId == quizId);
        }

        public async Task<Chapter?> GetChapterByQuizIdForUserAsync(int quizId, int userId)
        {
            // Prefer the chapter in a course the user is enrolled in (Approved)
            var fromEnrollment = await context.Chapters
                .Where(c => c.QuizId == quizId)
                .FirstOrDefaultAsync(c =>
                    context.CourseEnrollments.Any(e =>
                        e.UserId == userId &&
                        e.Status == EnrollmentStatus.Approved &&
                        e.Course.Modules.Any(m =>
                            m.Chapters.Any(ch => ch.ChapterId == c.ChapterId))));

            if (fromEnrollment is not null) return fromEnrollment;

            // Fall back: chapter in a course the user owns (creator testing their own quiz)
            return await context.Chapters
                .Where(c => c.QuizId == quizId)
                .FirstOrDefaultAsync(c =>
                    context.Courses.Any(co =>
                        co.UserId == userId &&
                        co.Modules.Any(m =>
                            m.Chapters.Any(ch => ch.ChapterId == c.ChapterId))));
        }

        public async Task<bool> UserHasEnrolledCourseWithQuizAsync(int userId, int quizId)
        {
            var fromChapter = await context.Chapters
                .AnyAsync(c => c.QuizId == quizId &&
                    context.CourseEnrollments.Any(e =>
                        e.UserId == userId &&
                        e.Status == EnrollmentStatus.Approved &&
                        e.Course.Modules.Any(m => m.Chapters.Any(ch => ch.ChapterId == c.ChapterId))));

            if (fromChapter) return true;

            return await context.Modules
                .AnyAsync(m => m.QuizId == quizId &&
                    context.CourseEnrollments.Any(e =>
                        e.UserId == userId &&
                        e.Status == EnrollmentStatus.Approved &&
                        e.CourseId == m.CourseId));
        }
    }
}
