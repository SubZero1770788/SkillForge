using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class GameViewModel
    {
        public int QuizId { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new();
    }
}