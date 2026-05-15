using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Entities
{
    public class QuizAttempt
    {
        public int QuizAttemptId { get; set; }

        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int Score { get; set; }

        public List<AnswerSelection> AnswerSelections { get; set; } = new();
    }

}