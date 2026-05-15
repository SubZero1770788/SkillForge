using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IQuizService
    {
        public Task<(bool success, string error)> DeleteAsync(int quizId);
        public Task<(bool success, string error)> CreateAsync(QuizViewModel quizViewModel, int userId);
        public Task<QuizViewModel> GetEditAsync(int quizId);
        public Task<(bool success, IEnumerable<string> errors)> PostEditAsync(QuizViewModel quizViewModel, int userId);
    }
}