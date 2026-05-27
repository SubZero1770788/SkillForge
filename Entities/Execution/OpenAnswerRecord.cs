using quiz_project.Entities.Definition;

namespace quiz_project.Entities
{
    public class OpenAnswerRecord
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = null!;

        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; } = null!;

        public string OpenText { get; set; } = "";

        public int? ManualScore { get; set; }

        public bool IsGraded { get; set; } = false;
    }
}
