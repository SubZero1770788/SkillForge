namespace quiz_project.Entities
{
    public class UserPartProgress
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChapterId { get; set; }
        public Chapter Chapter { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
