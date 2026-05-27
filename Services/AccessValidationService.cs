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
using quiz_project.Entities.Definition;
using quiz_project.Entities.Repositories;
using quiz_project.Extensions;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class AccessValidationService(IQuizRepository quizRepository, ICourseRepository courseRepository,
        IModuleRepository moduleRepository) : IAccessValidationService
    {
        public List<string> EachQuestionHasAnswer(QuizViewModel quizViewModel)
        {
            var errors = new List<string>();
            for (int i = 0; i < quizViewModel.Questions.Count; i++)
            {
                var q = quizViewModel.Questions[i];

                if (q.Type == QuestionType.FillInTheBlank || q.Type == QuestionType.OpenWithImage)
                    continue;

                if (q.Answers.Count < 2)
                    errors.Add($"Question {i + 1}: closed questions must have at least 2 answers.");

                if (!q.Answers.Any(a => a.IsCorrect))
                    errors.Add($"Question {i + 1}: must have at least one answer marked as correct.");
            }
            return errors;
        }

        public async Task<bool> UserOwnsQuizAsync(int id, User user)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(id);
            return quiz != null && quiz.UserId == user.Id;
        }

        public async Task<bool> UserOwnsCourseAsync(int id, User user)
        {
            var course = await courseRepository.GetCourseByIdAsync(id);
            return course != null && course.UserId == user.Id;
        }

        public async Task<bool> UserOwnsModuleAsync(int moduleId, User user)
        {
            var module = await moduleRepository.GetModuleByIdAsync(moduleId);
            if (module is null) return false;
            var course = await courseRepository.GetCourseByIdAsync(module.CourseId);
            return course != null && course.UserId == user.Id;
        }
    }
}