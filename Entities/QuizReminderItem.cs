using System;

namespace quiz_project.Entities
{
    public class QuizReminderItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public int IntervalIndex { get; set; } = 0;

        public DateTime NextReviewDate { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
