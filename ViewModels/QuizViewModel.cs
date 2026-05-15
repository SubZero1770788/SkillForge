using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using quiz_project.Entities;

namespace quiz_project.ViewModels
{
    public class QuizViewModel
    {
        public int QuizId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new();
        public int QuestionCount => Questions.Count;
        public int TotalScore => Questions.Sum(q => q.QuestionScore);
    }
}