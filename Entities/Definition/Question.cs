using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using quiz_project.Entities.Definition;

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
        public List<Answer> Answers { get; set; } = new();
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
        public GradingMethod? Grading { get; set; }
        public string? Keywords { get; set; }
        public string? ImagePath { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
        [NotMapped]
        public int Index { get; set; }
    }
}