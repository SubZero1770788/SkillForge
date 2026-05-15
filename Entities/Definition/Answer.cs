using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Entities
{
    public class Answer
    {
        public int AnswerId { get; set; }
        public required string Description { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}