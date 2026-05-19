using System.Collections.Generic;
using System.Threading.Tasks;
using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IAttemptRepository
    {
        public Task CreateAsync(QuizAttempt qa);
        public Task<IEnumerable<QuizAttempt>> GetAllAttemptsAsync(int quizId);
        public Task<QuizAttempt?> GetLatestUserAttemptAsync(int userId, int quizId);
        public Task<QuizAttempt?> GetTopUserAttemptAsync(int userId, int quizId);
        public Task<List<QuizAttempt>> GetBestUserAttemptsForQuizzesAsync(int userId, IEnumerable<int> quizIds);
        public Task<List<QuizAttempt>> GetAttemptsWithUngradedAnswersAsync(IEnumerable<int> quizIds);
        public Task<QuizAttempt?> GetAttemptWithOpenAnswersAsync(int attemptId);
        public Task<QuizAttempt?> GetAttemptFullDetailAsync(int attemptId);
        public Task<List<QuizAttempt>> GetUserAttemptsForQuizzesAsync(int userId, IEnumerable<int> quizIds);
        public Task<List<QuizAttempt>> GetAllBestAttemptsForUserAsync(int userId);
        public Task UpdateScoreAsync(int attemptId, int newScore);
        public Task SaveOpenAnswerGradesAsync(List<OpenAnswerRecord> records);
    }
}