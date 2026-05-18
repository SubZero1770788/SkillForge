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

        /// <summary>Determines how the question is rendered and scored.</summary>
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;

        /// <summary>
        /// For FillInTheBlank/OpenWithImage questions: how the answer is graded.
        /// Null for MultipleChoice/SingleChoice.
        /// </summary>
        public GradingMethod? Grading { get; set; }

        /// <summary>
        /// Comma-separated keywords used for automatic grading of FillInTheBlank
        /// and OpenWithImage (Keywords grading method).
        /// </summary>
        public string? Keywords { get; set; }

        /// <summary>R2 object key for the image shown in OpenWithImage questions.</summary>
        public string? ImagePath { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }
        [NotMapped]
        public int Index { get; set; }
    }
}