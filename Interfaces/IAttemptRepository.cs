using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IAttemptRepository
    {
        public Task CreateAsync(QuizAttempt qa);
        public Task<IEnumerable<QuizAttempt>> GetAllAttemptsAsync(int quizId);
        public Task<QuizAttempt> GetLatestUserAttemptAsync(int userId);
        public Task<QuizAttempt> GetTopUserAttemptAsync(int userId, int quizId);
    }
}