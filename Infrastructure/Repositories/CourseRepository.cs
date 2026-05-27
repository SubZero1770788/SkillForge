using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Entities.Repositories
{
    public class CourseRepository(QuizDb context) : ICourseRepository
    {
        public async Task<IEnumerable<Course>> GetCoursesByUserAsync(int userId)
        {
            var courses = await context.Courses.Where(x => x.UserId == userId)
                                        .Include(c => c.Modules).ToListAsync();
            return courses;
        }
        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            var courses = await context.Courses.ToListAsync();
            return courses;
        }
        public async Task<IEnumerable<Course>> GetPublicCourses()
        {
            var courses = await context.Courses.Where(c => c.IsPublic == true).Include(c => c.Modules).ToListAsync();
            return courses;
        }

        public async Task<bool> PublicTitleExistsAsync(string title, int excludeCourseId = 0)
        {
            return await context.Courses.AnyAsync(c =>
                c.IsPublic &&
                c.CourseId != excludeCourseId &&
                c.Title.ToLower() == title.ToLower());
        }
        public async Task CreateCourseAsync(Course course)
        {
            await context.AddAsync(course);
            await context.SaveChangesAsync();
        }
        public async Task DeleteCourseAsync(Course course)
        {
            var oldCourse = await context.Courses.Where(c => c.CourseId == course.CourseId).FirstAsync();
            context.Courses.Remove(oldCourse);
            await context.SaveChangesAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await context.Courses.Where(c => c.CourseId == courseId)
                                        .Include(c => c.Modules).FirstAsync();
            return course;
        }
        public async Task UpdateCourseAsync(Course course)
        {
            var existing = await context.Courses.FirstAsync(c => c.CourseId == course.CourseId);
            context.Entry(existing).CurrentValues.SetValues(course);
            await context.SaveChangesAsync();
        }

        public async Task<List<Module>> GetModulesByCourseId(int courseId)
        {
            return await context.Modules
                .Where(m => m.CourseId == courseId)
                .OrderBy(m => m.Order)
                .ToListAsync();
        }
    }
}