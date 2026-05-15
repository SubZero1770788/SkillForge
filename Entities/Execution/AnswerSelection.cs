using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Entities
{
    public class AnswerSelection
    {
        public int AnswerSelectionId { get; set; }

        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
    }
}