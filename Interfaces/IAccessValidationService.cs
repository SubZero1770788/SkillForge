using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IAccessValidationService
    {
        public Task<bool> UserOwnsQuizAsync(int id, User user);
        public List<string> EachQuestionHasAnswer(QuizViewModel quizViewModel);
    }
}