using System.Collections.Generic;
using quiz_project.Entities.Definition;

namespace quiz_project.ViewModels
{
    public class EnrolledUserItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
    }

    public class UserAttemptsViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = "";
        public List<UserAttemptItem> Attempts { get; set; } = new();
    }

    public class UserAttemptItem
    {
        public int AttemptId { get; set; }
        public string QuizTitle { get; set; } = "";
        public string Context { get; set; } = ""; 
        public int Score { get; set; }
        public int TotalScore { get; set; }
        public double ScorePercent => TotalScore == 0 ? 0 : System.Math.Round((double)Score / TotalScore * 100, 1);
        public bool HasPendingGrading { get; set; }
    }

    public class AttemptDetailViewModel
    {
        public int AttemptId { get; set; }
        public string UserName { get; set; } = "";
        public string QuizTitle { get; set; } = "";
        public int EarnedScore { get; set; }
        public int TotalScore { get; set; }
        public double ScorePercent => TotalScore == 0 ? 0 : System.Math.Round((double)EarnedScore / TotalScore * 100, 1);
        public List<AttemptQuestionDetail> Questions { get; set; } = new();
        public int? CourseId { get; set; }
        public int UserId { get; set; }
    }

    public class AttemptQuestionDetail
    {
        public string Description { get; set; } = "";
        public QuestionType Type { get; set; }
        public string? ImagePath { get; set; }
        public int MaxScore { get; set; }
        public int EarnedScore { get; set; }
        public List<AttemptAnswerOption> Options { get; set; } = new();
        public string? OpenText { get; set; }
        public bool IsGraded { get; set; }
        public int? ManualScore { get; set; }
        public string? Keywords { get; set; }
    }

    public class AttemptAnswerOption
    {
        public string Description { get; set; } = "";
        public bool WasSelected { get; set; }
        public bool IsCorrect { get; set; }
    }
}
