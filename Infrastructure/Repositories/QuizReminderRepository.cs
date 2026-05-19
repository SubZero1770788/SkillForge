using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class QuizReminderRepository(QuizDb context) : IQuizReminderRepository
    {
        public async Task<QuizReminderItem?> GetAsync(int userId, int quizId)
            => await context.QuizReminderItems
                .Include(r => r.Quiz)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.QuizId == quizId);

        public async Task<List<QuizReminderItem>> GetByUserAsync(int userId)
            => await context.QuizReminderItems
                .Include(r => r.Quiz)
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.NextReviewDate)
                .ToListAsync();

        public async Task<int> GetDueCountAsync(int userId)
            => await context.QuizReminderItems
                .CountAsync(r => r.UserId == userId && r.NextReviewDate <= DateTime.UtcNow);

        public async Task AddAsync(QuizReminderItem item)
        {
            await context.QuizReminderItems.AddAsync(item);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(QuizReminderItem item)
        {
            var existing = await context.QuizReminderItems.FindAsync(item.Id);
            if (existing is null) return;
            existing.IntervalIndex = item.IntervalIndex;
            existing.NextReviewDate = item.NextReviewDate;
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id, int userId)
        {
            var item = await context.QuizReminderItems
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (item is null) return;
            context.QuizReminderItems.Remove(item);
            await context.SaveChangesAsync();
        }
    }
}
