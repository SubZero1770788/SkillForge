using System;

namespace quiz_project.Entities
{
    /// <summary>
    /// Tracks where a user is in the spaced-repetition schedule for a quiz.
    /// Intervals (days between reviews): 3, 7, 14, 24, 35, 46, 57, 68, ... (+11 after index 5)
    /// Passing a review advances IntervalIndex; failing resets it to 0.
    /// </summary>
    public class QuizReminderItem
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; } = null!;

        /// <summary>Current position in the interval sequence (0 = 3-day step).</summary>
        public int IntervalIndex { get; set; } = 0;

        public DateTime NextReviewDate { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
