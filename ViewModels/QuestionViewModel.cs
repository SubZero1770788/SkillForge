using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using quiz_project.Entities.Definition;

namespace quiz_project.ViewModels
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public int? Index { get; set; }
        [Required]
        public string Description { get; set; }
        public int QuestionScore { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new();
        public List<int> SelectedAnswerIds { get; set; } = new();
        public string? OpenText { get; set; }
        public QuestionType Type { get; set; } = QuestionType.MultipleChoice;
        public GradingMethod? Grading { get; set; }
        public string? Keywords { get; set; }
        public string? ImagePath { get; set; }

        public bool IsDeleted { get; set; }
    }
}