using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.Services
{
    public class QuizGameService(IQuizRepository quizRepository, UserManager<User> userManager, IQuizMapper quizMapper,
                                    IOnGoingQuizRepository onGoingQuizRepository, IAttemptRepository attemptRepository,
                                    IAccessValidationService accessValidationService, IQuizQueryService quizQueryService) : IQuizGameService
    {
        public async Task<(bool, QuizSummaryViewModel?)> AttemptSummary(int quizId, User user)
        {
            var quizMetaData = await onGoingQuizRepository.GetAsync(user.Id, quizId);
            var userRole = await userManager.GetRolesAsync(user);

            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !userRole.Contains("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return (false, null);
            }

            var allQuizAttempts = await attemptRepository.GetAllAttemptsAsync(quizId);
            var quizDefinition = await quizRepository.GetQuizByIdAsync(quizId);

            var playerScore = await attemptRepository.GetLatestUserAttemptAsync(user.Id);

            var topScores = allQuizAttempts.OrderBy(aqa => aqa.Score).Take(10).ToList();
            var users = await userManager.Users.ToDictionaryAsync(u => u.Id, u => u.UserName);

            var quizSummaryViewModel = quizMapper.ToQuizSummaryViewModel(quizDefinition, topScores, users, playerScore);

            return (true, quizSummaryViewModel);
        }

        public async Task<QuizViewModel> LaunchQuizAsync(int quizId)
        {

            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            var quizViewModel = quizMapper.ToQuizViewModel(quiz);

            return quizViewModel;
        }
    }
}