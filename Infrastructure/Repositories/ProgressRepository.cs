using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class ProgressRepository(QuizDb context) : IProgressRepository
    {
        public async Task<UserModuleProgress?> GetModuleProgressAsync(int userId, int moduleId)
        {
            return await context.UserModuleProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == moduleId);
        }

        public async Task<UserPartProgress?> GetChapterProgressAsync(int userId, int chapterId)
        {
            return await context.UserPartProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ChapterId == chapterId);
        }

        public async Task<IEnumerable<UserPartProgress>> GetChapterProgressesForModuleAsync(int userId, int moduleId)
        {
            return await context.UserPartProgresses
                .Where(p => p.UserId == userId && p.Chapter.ModuleId == moduleId)
                .ToListAsync();
        }

        public async Task MarkChapterCompletedAsync(int userId, int chapterId)
        {
            var progress = await context.UserPartProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ChapterId == chapterId);

            if (progress is null)
            {
                await context.UserPartProgresses.AddAsync(new UserPartProgress
                {
                    UserId = userId,
                    ChapterId = chapterId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                });
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
        }

        public async Task MarkModuleCompletedAsync(int userId, int moduleId)
        {
            var progress = await context.UserModuleProgresses
                .FirstOrDefaultAsync(p => p.UserId == userId && p.ModuleId == moduleId);

            if (progress is null)
            {
                await context.UserModuleProgresses.AddAsync(new UserModuleProgress
                {
                    UserId = userId,
                    ModuleId = moduleId,
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow
                });
            }
            else
            {
                progress.IsCompleted = true;
                progress.CompletedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
        }
    }
}
