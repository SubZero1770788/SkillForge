using System.Collections.Generic;

namespace quiz_project.ViewModels
{
    public class PendingGradingViewModel
    {
        public List<PendingAttemptItem> Attempts { get; set; } = new();
    }

    public class PendingAttemptItem
    {
        public int AttemptId { get; set; }
        public string QuizTitle { get; set; } = "";
        public string UserName { get; set; } = "";
        public int UngradedCount { get; set; }
        public int TotalOpenAnswers { get; set; }
    }

    public class GradeAttemptViewModel
    {
        public int AttemptId { get; set; }
        public string QuizTitle { get; set; } = "";
        public string UserName { get; set; } = "";
        public int CurrentScore { get; set; }
        public List<OpenAnswerGradeItem> Answers { get; set; } = new();
    }

    public class OpenAnswerGradeItem
    {
        public int OpenAnswerRecordId { get; set; }
        public string QuestionDescription { get; set; } = "";
        public string? ImagePath { get; set; }
        public int MaxScore { get; set; }
        public string OpenText { get; set; } = "";
        public int? ManualScore { get; set; }
        public bool IsGraded { get; set; }
    }
}
