using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentStatus?> GetStatusAsync(int courseId, int userId);
        Task<EnrollmentStatus> EnrollAsync(int courseId, int userId, bool courseIsPaid);
        Task ApproveAsync(int enrollmentId);
        Task RejectAsync(int enrollmentId);
        Task<EnrollmentListViewModel> GetEnrollmentsAsync(int courseId, string courseTitle);
        Task<List<EnrolledCourseViewModel>> GetUserEnrolledCoursesAsync(int userId);
    }
}
