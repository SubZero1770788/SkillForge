using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<CourseEnrollment?> GetAsync(int courseId, int userId);
        Task<List<CourseEnrollment>> GetByCourseAsync(int courseId);
        Task<CourseEnrollment?> GetByIdAsync(int enrollmentId);
        Task CreateAsync(CourseEnrollment enrollment);
        Task UpdateAsync(CourseEnrollment enrollment);
    }
}
