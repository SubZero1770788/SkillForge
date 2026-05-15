using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public bool IsDeleted { get; set; }
    }
}