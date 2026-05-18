using quiz_project.Entities;

namespace quiz_project.ViewModels
{
    public class EnrolledCourseViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
