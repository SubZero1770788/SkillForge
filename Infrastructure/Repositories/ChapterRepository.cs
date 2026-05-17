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
    }
}
