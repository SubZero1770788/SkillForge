using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities.Definition;

namespace quiz_project.Entities
{
    public class OnGoingQuizQuestion
    {
        public int Id { get; set; }

        public int OnGoingQuizStateId { get; set; }
        public OnGoingQuizState OnGoingQuizState { get; set; }

        public int QuestionId { get; set; }

        public int Order { get; set; }
    }
}