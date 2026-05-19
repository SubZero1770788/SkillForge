using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class CreatorApplicationRepository(QuizDb context) : ICreatorApplicationRepository
    {
        public async Task<CreatorApplication?> GetByUserIdAsync(int userId)
            => await context.CreatorApplications
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId);

        public async Task<List<CreatorApplication>> GetAllAsync()
            => await context.CreatorApplications
                .Include(a => a.User)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

        public async Task<List<CreatorApplication>> GetByStatusAsync(ApplicationStatus status)
            => await context.CreatorApplications
                .Include(a => a.User)
                .Where(a => a.Status == status)
                .OrderByDescending(a => a.AppliedAt)
                .ToListAsync();

        public async Task<CreatorApplication?> GetByIdAsync(int id)
            => await context.CreatorApplications
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

        public async Task AddAsync(CreatorApplication application)
        {
            await context.CreatorApplications.AddAsync(application);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CreatorApplication application)
        {
            var existing = await context.CreatorApplications.FindAsync(application.Id);
            if (existing is null) return;
            existing.Status = application.Status;
            existing.ReviewedAt = application.ReviewedAt;
            existing.ReviewNote = application.ReviewNote;
            await context.SaveChangesAsync();
        }
    }
}
