using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.ViewModels
{
    public class AnswerViewModel
    {
        public int AnswerId { get; set; }
        [Required]
        public string Description { get; set; }
        public bool IsCorrect { get; set; }
    }
}