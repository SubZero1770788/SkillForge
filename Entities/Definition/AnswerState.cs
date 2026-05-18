using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Entities.Definition
{
    public class AnswerState
    {
        public int Id { get; set; }
        public int OnGoingQuizStateId { get; set; }
        public OnGoingQuizState OnGoingQuizState { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public List<int> AnswersId { get; set; } = new();

        /// <summary>User's text response for FillInTheBlank or OpenWithImage questions.</summary>
        public string? OpenText { get; set; }
    }
}