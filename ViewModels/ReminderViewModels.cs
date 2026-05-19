using System;
using System.Collections.Generic;

namespace quiz_project.ViewModels
{
    public class MyQuizzesViewModel
    {
        public List<AttemptedQuizItem> Quizzes { get; set; } = new();
    }

    public class AttemptedQuizItem
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = "";
        public int BestScore { get; set; }
        public int TotalScore { get; set; }
        public double BestPercent => TotalScore == 0 ? 0 : Math.Round((double)BestScore / TotalScore * 100, 1);
        public bool Passed { get; set; }

        // Reminder state
        public bool HasReminder { get; set; }
        public int? ReminderId { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public bool IsDue => NextReviewDate.HasValue && NextReviewDate.Value <= DateTime.UtcNow;
        public int IntervalIndex { get; set; }
        public int CurrentIntervalDays { get; set; }
    }
}
