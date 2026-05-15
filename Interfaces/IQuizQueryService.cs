using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IQuizQueryService
    {
        Task<List<QuizViewModel>> GetUserQuizzesAsync(int userId);
        Task<(bool, QuizStatisticsModel?)> GetQuizStatisticsAsync(int quizId, int userId);
        Task<bool> CheckIfPublicAsync(int Id);
    }
}