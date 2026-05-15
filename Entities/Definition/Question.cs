using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Entities
{
    public class Question
    {
        public int QuestionId { get; set; }
        [Required]
        public string Description { get; set; }
        public int QuestionScore { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        [Range(0, 100)]
        public List<Answer> Answers { get; set; } = new();
        [NotMapped]
        public bool IsDeleted { get; set; }
        [NotMapped]
        public int Index { get; set; }
    }
}