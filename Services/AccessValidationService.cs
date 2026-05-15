using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using quiz_project.Common;
using quiz_project.Entities;
using quiz_project.Entities.Repositories;
using quiz_project.Extensions;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class AccessValidationService(IQuizRepository quizRepository) : IAccessValidationService
    {
        public List<string> EachQuestionHasAnswer(QuizViewModel quizViewModel)
        {
            var errors = new List<string>();
            for (int i = 0; i < quizViewModel.Questions.Count; i++)
            {
                if (!quizViewModel.Questions[i].Answers.Any(a => a.IsCorrect))
                    errors.Add($"Questions[{i}].Answers : Question {i + 1} must have at least one correct answer.");
            }
            return errors;
        }

        public async Task<bool> UserOwnsQuizAsync(int id, User user)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(id);
            return quiz != null && quiz.UserId == user.Id;
        }

    }
}