using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IQuizRepository
    {
        Task<IEnumerable<Quiz>> GetQuizesAsync();
        Task<IEnumerable<Quiz>> GetQuizesByUserAsync(int userId);
        Task<IEnumerable<Quiz>> GetPublicQuizzes();
        Task<Quiz> GetQuizByIdAsync(int quizId);
        Task DeleteQuizAsync(Quiz quiz);
        Task UpdateQuizAsync(Quiz quiz);
        Task CreateQuizAsync(Quiz quiz);
        Task<List<Question>> GetQuestionsByQuizId(int quizId);
        Task<Dictionary<(int QuestionId, int AnswerId), int>> GetAnswerSelectionStatsAsync(int quizId);
    }
}