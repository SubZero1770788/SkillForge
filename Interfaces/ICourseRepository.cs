using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByUserAsync(int userId);
        Task<IEnumerable<Course>> GetPublicCourses();
        Task<Course> GetCourseByIdAsync(int courseId);
        Task DeleteCourseAsync(Course course);
        Task UpdateCourseAsync(Course course);
        Task CreateCourseAsync(Course course);
        Task<List<Module>> GetModulesByCourseId(int courseId);
        //Task<Dictionary<(int QuestionId, int AnswerId), int>> GetAnswerSelectionStatsAsync(int courseId);
    }
}