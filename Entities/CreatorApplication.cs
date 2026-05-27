using System;
using quiz_project.Entities.Definition;

namespace quiz_project.Entities
{
    public class CreatorApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string ContactName { get; set; } = "";
        public string Description { get; set; } = "";

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
    }
}
