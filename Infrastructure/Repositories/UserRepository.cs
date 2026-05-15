using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class UserRepository(QuizDb context) : IUserRepository
    {
        public async Task<Dictionary<int, int>> GetTotalScoresAsync()
        {
            var scores = await context.QuizAttempts
                .GroupBy(u => u.UserId).Select(u => new
                {
                    UserId = u.Key,
                    TotalScore = u.Sum(a => a.Score)
                }).ToDictionaryAsync(u => u.UserId, ts => ts.TotalScore);

            return scores;
        }
    }
}