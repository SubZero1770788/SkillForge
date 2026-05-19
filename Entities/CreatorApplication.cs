using System;
using quiz_project.Entities.Definition;

namespace quiz_project.Entities
{
    public class CreatorApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        /// <summary>Full name or company name of the applicant.</summary>
        public string ContactName { get; set; } = "";

        /// <summary>What the creator plans to publish on the platform.</summary>
        public string Description { get; set; } = "";

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }

        /// <summary>Admin note shown to the applicant on rejection.</summary>
        public string? ReviewNote { get; set; }
    }
}
