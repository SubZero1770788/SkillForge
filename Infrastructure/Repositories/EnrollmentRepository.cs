using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class EnrollmentRepository(QuizDb context) : IEnrollmentRepository
    {
        public async Task<CourseEnrollment?> GetAsync(int courseId, int userId)
        {
            return await context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.UserId == userId);
        }

        public async Task<List<CourseEnrollment>> GetByCourseAsync(int courseId)
        {
            return await context.CourseEnrollments
                .Include(e => e.User)
                .Where(e => e.CourseId == courseId)
                .OrderBy(e => e.EnrolledAt)
                .ToListAsync();
        }

        public async Task<List<CourseEnrollment>> GetByUserAsync(int userId)
        {
            return await context.CourseEnrollments
                .Include(e => e.Course)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();
        }

        public async Task<CourseEnrollment?> GetByIdAsync(int enrollmentId)
        {
            return await context.CourseEnrollments
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.CourseEnrollmentId == enrollmentId);
        }

        public async Task CreateAsync(CourseEnrollment enrollment)
        {
            await context.CourseEnrollments.AddAsync(enrollment);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CourseEnrollment enrollment)
        {
            context.CourseEnrollments.Update(enrollment);
            await context.SaveChangesAsync();
        }

        public async Task<Dictionary<int, int>> GetPendingCountByCourseIdsAsync(IEnumerable<int> courseIds)
        {
            var ids = courseIds.ToList();
            return await context.CourseEnrollments
                .Where(e => ids.Contains(e.CourseId) && e.Status == EnrollmentStatus.Pending)
                .GroupBy(e => e.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.CourseId, x => x.Count);
        }
    }
}
