using System;
using System.Collections.Generic;
using quiz_project.Entities.Definition;

namespace quiz_project.ViewModels
{

    public class AdminUserListViewModel
    {
        public List<AdminUserItem> Users { get; set; } = new();
        public int PendingApplicationsCount { get; set; }
    }

    public class AdminUserItem
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public bool IsLockedOut { get; set; }

        public bool IsUser => Roles.Contains("User");
        public bool IsCreator => Roles.Contains("Creator");
        public bool IsAdmin => Roles.Contains("Admin");
    }

    public class AdminApplicationsViewModel
    {
        public List<CreatorApplicationItem> Pending { get; set; } = new();
        public List<CreatorApplicationItem> Reviewed { get; set; } = new();
    }

    public class CreatorApplicationItem
    {
        public int ApplicationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string ContactName { get; set; } = "";
        public string Description { get; set; } = "";
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
    }
    public class ReviewApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public string? ReviewNote { get; set; }
    }
}
