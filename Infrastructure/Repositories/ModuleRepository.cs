using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class ModuleRepository(QuizDb context) : IModuleRepository
    {
        public async Task<Module?> GetModuleByIdAsync(int moduleId)
        {
            return await context.Modules
                .Include(m => m.Chapters)
                .FirstOrDefaultAsync(m => m.ModuleId == moduleId);
        }

        public async Task<IEnumerable<Module>> GetModulesByCourseIdAsync(int courseId)
        {
            return await context.Modules
                .Where(m => m.CourseId == courseId)
                .Include(m => m.Chapters)
                .OrderBy(m => m.Order)
                .ToListAsync();
        }

        public async Task CreateModuleAsync(Module module)
        {
            await context.Modules.AddAsync(module);
            await context.SaveChangesAsync();
        }

        public async Task UpdateModuleAsync(Module module)
        {
            var existing = await context.Modules.FirstAsync(m => m.ModuleId == module.ModuleId);
            context.Entry(existing).CurrentValues.SetValues(module);
            await context.SaveChangesAsync();
        }

        public async Task DeleteModuleAsync(Module module)
        {
            var existing = await context.Modules.FirstAsync(m => m.ModuleId == module.ModuleId);
            context.Modules.Remove(existing);
            await context.SaveChangesAsync();
        }
    }
}
