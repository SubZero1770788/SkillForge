using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface ICourseService
    {
        public Task<(bool success, string error)> DeleteAsync(int courseId);
        public Task<(bool success, int courseId, string error)> CreateAsync(CourseViewModel courseViewModel, int userId);
        public Task<CourseViewModel> GetEditAsync(int courseId);
        public Task<(bool success, IEnumerable<string> errors)> PostEditAsync(CourseViewModel courseViewModel, int userId);
    }
}