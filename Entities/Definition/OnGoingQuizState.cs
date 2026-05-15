using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Entities.Definition
{
    public class OnGoingQuizState
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public int CurrentPage { get; set; }
        public int QuestionCount { get; set; }
        public List<AnswerState> Answers { get; set; } = new();
    }
}