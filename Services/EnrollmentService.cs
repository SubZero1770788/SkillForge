using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class EnrollmentService(IEnrollmentRepository enrollmentRepository) : IEnrollmentService
    {
        public async Task<EnrollmentStatus?> GetStatusAsync(int courseId, int userId)
        {
            var enrollment = await enrollmentRepository.GetAsync(courseId, userId);
            return enrollment?.Status;
        }

        public async Task<EnrollmentStatus> EnrollAsync(int courseId, int userId, bool courseIsPaid)
        {
            var existing = await enrollmentRepository.GetAsync(courseId, userId);
            if (existing is not null) return existing.Status;

            var status = courseIsPaid ? EnrollmentStatus.Pending : EnrollmentStatus.Approved;

            await enrollmentRepository.CreateAsync(new CourseEnrollment
            {
                CourseId = courseId,
                UserId = userId,
                Status = status,
                EnrolledAt = DateTime.UtcNow
            });

            return status;
        }

        public async Task ApproveAsync(int enrollmentId)
        {
            var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment is null) return;
            enrollment.Status = EnrollmentStatus.Approved;
            await enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task RejectAsync(int enrollmentId)
        {
            var enrollment = await enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment is null) return;
            enrollment.Status = EnrollmentStatus.Rejected;
            await enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task<EnrollmentListViewModel> GetEnrollmentsAsync(int courseId, string courseTitle)
        {
            var enrollments = await enrollmentRepository.GetByCourseAsync(courseId);
            return new EnrollmentListViewModel
            {
                CourseId = courseId,
                CourseTitle = courseTitle,
                Enrollments = enrollments.Select(e => new EnrollmentItemViewModel
                {
                    EnrollmentId = e.CourseEnrollmentId,
                    UserName = e.User.UserName ?? "—",
                    Status = e.Status,
                    EnrolledAt = e.EnrolledAt
                }).ToList()
            };
        }
    }
}
