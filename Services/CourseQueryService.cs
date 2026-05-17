using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;
using static quiz_project.ViewModels.QuizSummaryViewModel;

namespace quiz_project.Services
{
    public class CourseQueryService(ICourseRepository courseRepository, ICourseMapper courseMapper,
                                //IAttemptRepository attemptRepository, 
                                UserManager<User> userManager) : ICourseQueryService
    {
        public async Task<bool> CheckIfPublicAsync(int Id)
        {
            var course = await courseRepository.GetCourseByIdAsync(Id);
            if (course.IsPublic) return true;
            return false;
        }

        public async Task<List<CourseViewModel>> GetUserCoursesAsync(int userId)
        {
            var courses = await courseRepository.GetCoursesByUserAsync(userId);
            return courses.Select(c => courseMapper.ToCourseViewModel(c)).ToList();
        }

        public async Task<List<CourseViewModel>> GetUserSignedUpCoursesAsync(int userId)
        {
            var courses = await courseRepository.GetCoursesByUserAsync(userId);
            return courses.Select(c => courseMapper.ToCourseViewModel(c)).ToList();
        }

        public async Task<List<CourseViewModel>> GetPublicCoursesAsync()
        {
            var courses = await courseRepository.GetPublicCourses();
            return courses.Select(c => courseMapper.ToCourseViewModel(c)).ToList();
        }
    }
}