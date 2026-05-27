using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.ViewModels.Mappers
{
    public class CourseMapper : ICourseMapper
    {

        public Course ToEntity(CourseViewModel courseViewModel, int userId)
        {
            return new Course
            {
                CourseId = courseViewModel.CourseId,
                Title = courseViewModel.Title,
                Description = courseViewModel.Description,
                UserId = userId,
                IsPublic = courseViewModel.IsPublic,
                IsSequential = courseViewModel.IsSequential,
                IsPaid = courseViewModel.IsPaid
            };
        }

        public CourseViewModel ToCourseViewModel(Course course)
        {
            return new CourseViewModel
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                IsPublic = course.IsPublic,
                IsSequential = course.IsSequential,
                IsPaid = course.IsPaid
            };
        }
    }
}