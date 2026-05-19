using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Database.Repositories
{
    public class AttemptRepository(QuizDb context) : IAttemptRepository
    {
        public async Task CreateAsync(QuizAttempt qa)
        {
            await context.AddAsync(qa);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<QuizAttempt>> GetAllAttemptsAsync(int quizId)
        {
            var attempt = await context.QuizAttempts.Where(qa => qa.QuizId == quizId).ToListAsync();
            return attempt;
        }

        public async Task<QuizAttempt?> GetLatestUserAttemptAsync(int userId, int quizId)
        {
            return await context.QuizAttempts
                .Include(qa => qa.OpenAnswerRecords)
                .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
                .OrderByDescending(qa => qa.QuizAttemptId)
                .FirstOrDefaultAsync();
        }

        public async Task<QuizAttempt?> GetTopUserAttemptAsync(int userId, int quizId)
        {
            return await context.QuizAttempts
                .Where(qa => qa.UserId == userId && qa.QuizId == quizId)
                .OrderByDescending(qa => qa.Score)
                .FirstOrDefaultAsync();
        }

        public async Task<List<QuizAttempt>> GetBestUserAttemptsForQuizzesAsync(int userId, IEnumerable<int> quizIds)
        {
            var ids = quizIds.ToList();
            return await context.QuizAttempts
                .Include(qa => qa.Quiz)
                .Where(qa => qa.UserId == userId && ids.Contains(qa.QuizId))
                .GroupBy(qa => qa.QuizId)
                .Select(g => g.OrderByDescending(qa => qa.Score).First())
                .ToListAsync();
        }

        public async Task<List<QuizAttempt>> GetAttemptsWithUngradedAnswersAsync(IEnumerable<int> quizIds)
        {
            var ids = quizIds.ToList();
            return await context.QuizAttempts
                .Include(qa => qa.Quiz)
                .Include(qa => qa.User)
                .Include(qa => qa.OpenAnswerRecords)
                .Where(qa => ids.Contains(qa.QuizId) &&
                             qa.OpenAnswerRecords.Any(oa => !oa.IsGraded))
                .ToListAsync();
        }

        public async Task<QuizAttempt?> GetAttemptWithOpenAnswersAsync(int attemptId)
        {
            return await context.QuizAttempts
                .Include(qa => qa.Quiz)
                .Include(qa => qa.User)
                .Include(qa => qa.OpenAnswerRecords)
                    .ThenInclude(oa => oa.Question)
                .FirstOrDefaultAsync(qa => qa.QuizAttemptId == attemptId);
        }

        public async Task UpdateScoreAsync(int attemptId, int newScore)
        {
            var attempt = await context.QuizAttempts.FirstOrDefaultAsync(a => a.QuizAttemptId == attemptId);
            if (attempt is null) return;
            attempt.Score = newScore;
            await context.SaveChangesAsync();
        }

        public async Task<QuizAttempt?> GetAttemptFullDetailAsync(int attemptId)
        {
            return await context.QuizAttempts
                .Include(qa => qa.Quiz).ThenInclude(q => q.Questions).ThenInclude(q => q.Answers)
                .Include(qa => qa.User)
                .Include(qa => qa.AnswerSelections)
                .Include(qa => qa.OpenAnswerRecords)
                .FirstOrDefaultAsync(qa => qa.QuizAttemptId == attemptId);
        }

        public async Task<List<QuizAttempt>> GetUserAttemptsForQuizzesAsync(int userId, IEnumerable<int> quizIds)
        {
            var ids = quizIds.ToList();
            return await context.QuizAttempts
                .Include(qa => qa.Quiz)
                .Include(qa => qa.OpenAnswerRecords)
                .Where(qa => qa.UserId == userId && ids.Contains(qa.QuizId))
                .OrderByDescending(qa => qa.QuizAttemptId)
                .ToListAsync();
        }

        public async Task<List<QuizAttempt>> GetAllBestAttemptsForUserAsync(int userId)
        {
            return await context.QuizAttempts
                .Include(qa => qa.Quiz)
                .Where(qa => qa.UserId == userId)
                .GroupBy(qa => qa.QuizId)
                .Select(g => g.OrderByDescending(qa => qa.Score).First())
                .ToListAsync();
        }

        public async Task SaveOpenAnswerGradesAsync(List<OpenAnswerRecord> records)
        {
            foreach (var record in records)
            {
                var existing = await context.OpenAnswerRecords.FirstOrDefaultAsync(r => r.Id == record.Id);
                if (existing is null) continue;
                existing.ManualScore = record.ManualScore;
                existing.IsGraded = record.IsGraded;
            }
            await context.SaveChangesAsync();
        }
    }
}