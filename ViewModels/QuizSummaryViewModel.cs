using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class QuizSummaryViewModel
    {
        public int Score { get; set; }
        public int TotalScore { get; set; }
        [Range(0, 10)]
        public List<TopScore> TopPlayerScores { get; set; }
        public List<QuestionStats> Questions { get; set; }

        public class TopScore
        {
            public string UserName { get; set; }
            public int PlayerScore { get; set; }
        }
        public class QuestionStats
        {
            public int QuestionId { get; set; }
            public string Description { get; set; }
            public List<AnswerStats> Answers { get; set; }
        }

        public class AnswerStats
        {
            public int AnswerId { get; set; }
            public string Description { get; set; }
            public int SelectedByCount { get; set; }
        }
    }
}