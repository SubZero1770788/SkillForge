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

        public async Task<QuizAttempt> GetLatestUserAttemptAsync(int userId)
        {
            var attempt = await context.QuizAttempts.Where(qa => qa.UserId == userId).OrderBy(qa => qa.QuizAttemptId).LastAsync();
            return attempt;
        }

        public async Task<QuizAttempt> GetTopUserAttemptAsync(int userId, int quizId)
        {
            var attempt = await context.QuizAttempts.Where(qa => qa.UserId == userId).OrderBy(qa => qa.Score).FirstAsync();
            return attempt;
        }
    }
}