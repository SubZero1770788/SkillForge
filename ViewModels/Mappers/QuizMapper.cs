using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.ViewModels.Mappers
{
    public class QuizMapper : IQuizMapper
    {
        public Quiz ToEntity(QuizViewModel quizViewModel, int userId)
        {
            var quiz = new Quiz
            {
                QuizId = quizViewModel.QuizId,
                Title = quizViewModel.Title,
                Description = quizViewModel.Description,
                TotalScore = quizViewModel.Questions.Sum(qvm => qvm.QuestionScore),
                UserId = userId,
                IsPublic = quizViewModel.IsPublic,
                Questions = quizViewModel.Questions.Select((qvm, index) => new Question
                {
                    QuizId = quizViewModel.QuizId,
                    QuestionId = qvm.QuestionId,
                    Index = qvm.Index ?? 0,
                    QuestionScore = qvm.QuestionScore,
                    Description = qvm.Description,
                    Answers = qvm.Answers.Select(avm => new Answer
                    {
                        QuestionId = qvm.QuestionId,
                        AnswerId = avm.AnswerId,
                        Description = avm.Description,
                        IsCorrect = avm.IsCorrect
                    }).ToList()
                }).ToList()
            };

            return quiz;
        }

        public QuizStatisticsModel ToQuizStatisticsModel(Quiz quiz, double averageScores, IEnumerable<QuizAttempt> allQuizAttempts,
                                QuizAttempt topUserAttempt, List<QuizAttempt> topScores,
                                Dictionary<int, string> users, Dictionary<(int QuestionId, int AnswerId), int>? answerCounts)
        {

            var quizStatisticsModel = new QuizStatisticsModel
            {
                Title = quiz.Title,
                AverageScore = averageScores,
                ScorePercentage = averageScores / quiz.TotalScore * 100,
                UsersFinished = allQuizAttempts.DistinctBy(aqa => aqa.UserId).Count(),
                quizSummaryViewModel = new QuizSummaryViewModel
                {
                    Score = topUserAttempt.Score,
                    TotalScore = quiz.TotalScore,
                    TopPlayerScores = topScores.Select(a => new TopScore
                    {
                        UserName = users[a.UserId] ?? "User not found",
                        PlayerScore = a.Score
                    }).OrderBy(a => a.PlayerScore).ToList(),

                    Questions = quiz.Questions.Select(q => new QuizSummaryViewModel.QuestionStats
                    {
                        QuestionId = q.QuestionId,
                        Description = q.Description,
                        Answers = q.Answers.Select(a => new QuizSummaryViewModel.AnswerStats
                        {
                            AnswerId = a.AnswerId,
                            Description = a.Description,
                            SelectedByCount = answerCounts?.GetValueOrDefault((q.QuestionId, a.AnswerId)) ?? 0
                        }).ToList()
                    }).ToList()
                }
            };

            return quizStatisticsModel;
        }

        public QuizSummaryViewModel ToQuizSummaryViewModel(Quiz quiz, List<QuizAttempt> topScores,
                                Dictionary<int, string> users, QuizAttempt playerScore)
        {
            QuizSummaryViewModel quizSummaryViewModel = new()
            {
                Score = playerScore.Score,
                TotalScore = quiz.TotalScore,
                TopPlayerScores = topScores.Select(a => new TopScore
                {
                    UserName = users[a.UserId] ?? "User not found",
                    PlayerScore = a.Score
                }).OrderBy(a => a.PlayerScore).ToList()
            };

            return quizSummaryViewModel;
        }

        public QuizViewModel ToQuizViewModel(Quiz quiz)
        {
            var quizViewModel = new QuizViewModel
            {
                IsPublic = quiz.IsPublic,
                QuizId = quiz!.QuizId,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions.Select((qvm, index) => new QuestionViewModel
                {
                    Index = index,
                    QuestionId = qvm.QuestionId != 0 ? qvm.QuestionId : 0,
                    QuestionScore = qvm.QuestionScore,
                    Description = qvm.Description,
                    Answers = qvm.Answers.Select(avm => new AnswerViewModel
                    {
                        AnswerId = avm.AnswerId,
                        Description = avm.Description,
                        IsCorrect = avm.IsCorrect
                    }).ToList()
                }).ToList()
            };

            return quizViewModel;
        }


    }
}