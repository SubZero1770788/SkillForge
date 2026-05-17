namespace quiz_project.Entities
{
    public class CourseEnrollment
    {
        public int CourseEnrollmentId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public EnrollmentStatus Status { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
