using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class QuizStatisticsModel
    {
        public string Title { get; set; }
        public double AverageScore { get; set; }
        public double ScorePercentage { get; set; }
        public int UsersFinished { get; set; }
        public QuizSummaryViewModel quizSummaryViewModel { get; set; }
    }
}