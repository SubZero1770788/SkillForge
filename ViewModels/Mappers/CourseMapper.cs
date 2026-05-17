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
    public class CourseMapper : ICourseMapper
    {
        // public Quiz ToEntity(QuizViewModel quizViewModel, int userId)
        // {
        //     var quiz = new Quiz
        //     {
        //         QuizId = quizViewModel.QuizId,
        //         Title = quizViewModel.Title,
        //         Description = quizViewModel.Description,
        //         TotalScore = quizViewModel.Questions.Sum(qvm => qvm.QuestionScore),
        //         UserId = userId,
        //         IsPublic = quizViewModel.IsPublic,
        //         Questions = quizViewModel.Questions.Select((qvm, index) => new Question
        //         {
        //             QuizId = quizViewModel.QuizId,
        //             QuestionId = qvm.QuestionId,
        //             Index = qvm.Index ?? 0,
        //             QuestionScore = qvm.QuestionScore,
        //             Description = qvm.Description,
        //             Answers = qvm.Answers.Select(avm => new Answer
        //             {
        //                 QuestionId = qvm.QuestionId,
        //                 AnswerId = avm.AnswerId,
        //                 Description = avm.Description,
        //                 IsCorrect = avm.IsCorrect
        //             }).ToList()
        //         }).ToList()
        //     };

        //     return quiz;
        // }

        public Course ToEntity(CourseViewModel courseViewModel, int userId)
        {
            return new Course
            {
                CourseId = courseViewModel.CourseId,
                Title = courseViewModel.Title,
                Description = courseViewModel.Description,
                UserId = userId,
                IsPublic = courseViewModel.IsPublic,
                IsSequential = courseViewModel.IsSequential,
                IsPaid = courseViewModel.IsPaid
            };
        }

        public CourseViewModel ToCourseViewModel(Course course)
        {
            return new CourseViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                IsPublic = course.IsPublic,
                IsSequential = course.IsSequential,
                IsPaid = course.IsPaid
            };
        }

        // public QuizStatisticsModel ToQuizStatisticsModel(Quiz quiz, double averageScores, IEnumerable<QuizAttempt> allQuizAttempts,
        //                         QuizAttempt topUserAttempt, List<QuizAttempt> topScores,
        //                         Dictionary<int, string> users, Dictionary<(int QuestionId, int AnswerId), int>? answerCounts)
        // {

        //     var quizStatisticsModel = new QuizStatisticsModel
        //     {
        //         Title = quiz.Title,
        //         AverageScore = averageScores,
        //         ScorePercentage = averageScores / quiz.TotalScore * 100,
        //         UsersFinished = allQuizAttempts.DistinctBy(aqa => aqa.UserId).Count(),
        //         quizSummaryViewModel = new QuizSummaryViewModel
        //         {
        //             Score = topUserAttempt.Score,
        //             TotalScore = quiz.TotalScore,
        //             TopPlayerScores = topScores.Select(a => new TopScore
        //             {
        //                 UserName = users[a.UserId] ?? "User not found",
        //                 PlayerScore = a.Score
        //             }).OrderBy(a => a.PlayerScore).ToList(),

        //             Questions = quiz.Questions.Select(q => new QuizSummaryViewModel.QuestionStats
        //             {
        //                 QuestionId = q.QuestionId,
        //                 Description = q.Description,
        //                 Answers = q.Answers.Select(a => new QuizSummaryViewModel.AnswerStats
        //                 {
        //                     AnswerId = a.AnswerId,
        //                     Description = a.Description,
        //                     SelectedByCount = answerCounts?.GetValueOrDefault((q.QuestionId, a.AnswerId)) ?? 0
        //                 }).ToList()
        //             }).ToList()
        //         }
        //     };

        //     return quizStatisticsModel;
        // }

        // public QuizSummaryViewModel ToQuizSummaryViewModel(Quiz quiz, List<QuizAttempt> topScores,
        //                         Dictionary<int, string> users, QuizAttempt playerScore)
        // {
        //     QuizSummaryViewModel quizSummaryViewModel = new()
        //     {
        //         Score = playerScore.Score,
        //         TotalScore = quiz.TotalScore,
        //         TopPlayerScores = topScores.Select(a => new TopScore
        //         {
        //             UserName = users[a.UserId] ?? "User not found",
        //             PlayerScore = a.Score
        //         }).OrderBy(a => a.PlayerScore).ToList()
        //     };

        //     return quizSummaryViewModel;
        // }
    }
}