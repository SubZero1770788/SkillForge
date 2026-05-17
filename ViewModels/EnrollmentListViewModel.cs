using quiz_project.Entities;

namespace quiz_project.ViewModels
{
    public class EnrollmentListViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public List<EnrollmentItemViewModel> Enrollments { get; set; } = new();
    }

    public class EnrollmentItemViewModel
    {
        public int EnrollmentId { get; set; }
        public string UserName { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
