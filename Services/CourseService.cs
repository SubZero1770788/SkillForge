using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class CourseService(ICourseRepository courseRepository, ICourseMapper courseMapper, IAccessValidationService accessValidationService) : ICourseService
    {
        public async Task<(bool success, string error)> CreateAsync(CourseViewModel courseViewModel, int userId)
        {
            //Kurs może być publiczny tylko gdy ma minimum 1 chapter
            var course = courseMapper.ToEntity(courseViewModel, userId);
            await courseRepository.CreateCourseAsync(course);
            return (true, String.Empty);
        }

        public async Task<(bool success, string error)> DeleteAsync(int courseId)
        {
            var course = await courseRepository.GetCourseByIdAsync(courseId);
            await courseRepository.DeleteCourseAsync(course);
            return (true, String.Empty);
        }

        public async Task<CourseViewModel> GetEditAsync(int courseId)
        {
            var course = await courseRepository.GetCourseByIdAsync(courseId);
            var courseViewModel = courseMapper.ToCourseViewModel(course);
            return courseViewModel;
        }

        public async Task<(bool success, IEnumerable<string> errors)> PostEditAsync(CourseViewModel courseViewModel, int userId)
        {
            // tutaj sprawdzenie czy ma chapter
            // if (courseViewModel.IsPublic && (courseViewModel < 50 || quizViewModel.QuestionCount < 5))
            // {
            //     return (false, new[]
            //     {
            //         "In order for quiz to be public it needs at least 5 questions with 50 combined points"
            //     });
            // }

            var course = courseMapper.ToEntity(courseViewModel, userId);

            await courseRepository.UpdateCourseAsync(course);
            return (true, Enumerable.Empty<string>());
        }

    }
}