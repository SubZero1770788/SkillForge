using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.Services
{
    public class QuizQueryService(IQuizRepository quizRepository, IQuizMapper quizMapper,
                                IAttemptRepository attemptRepository, UserManager<User> userManager) : IQuizQueryService
    {
        public async Task<bool> CheckIfPublicAsync(int Id)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(Id);
            return quiz?.IsPublic ?? false;
        }

        public async Task<bool> CheckIfRandomizedAsync(int Id)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(Id);
            if (quiz.IsRandomized) return true;
            return false;
        }

        public async Task<(bool, QuizStatisticsModel?)> GetQuizStatisticsAsync(int quizId, int userId)
        {
            var allQuizAttempts = await attemptRepository.GetAllAttemptsAsync(quizId);
            if (!allQuizAttempts.Any())
            {
                return (false, null);
            }

            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            var averageScores = allQuizAttempts.Average(aqa => aqa.Score);
            var topUserAttempt = await attemptRepository.GetTopUserAttemptAsync(userId, quiz.QuizId);
            var topScores = allQuizAttempts.OrderByDescending(aqa => aqa.Score).Take(10).ToList();
            var users = await userManager.Users.ToDictionaryAsync(u => u.Id, u => u.UserName);
            var answerCounts = await quizRepository.GetAnswerSelectionStatsAsync(quizId);

            var quizStatisticsModel = quizMapper.ToQuizStatisticsModel(
                quiz, averageScores, allQuizAttempts, topUserAttempt, topScores, users, answerCounts
            );
            return (true, quizStatisticsModel);
        }

        public async Task<List<QuizViewModel>> GetUserQuizzesAsync(int userId)
        {
            var quizes = await quizRepository.GetQuizesByUserAsync(userId);
            return quizes.Select(q => quizMapper.ToQuizViewModel(q)).ToList();
        }

        public async Task<List<QuizViewModel>> GetPublicQuizzesAsync()
        {
            var quizes = await quizRepository.GetPublicQuizzes();
            return quizes.Select(q => quizMapper.ToQuizViewModel(q)).ToList();
        }
    }
}