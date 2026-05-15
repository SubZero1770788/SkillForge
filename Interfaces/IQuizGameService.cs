using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IQuizGameService
    {
        public Task<(bool, QuizSummaryViewModel?)> AttemptSummary(int quizId, User user);
        public Task<QuizViewModel> LaunchQuizAsync(int quizId);
    }
}