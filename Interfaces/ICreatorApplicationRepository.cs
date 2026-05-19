using System.Collections.Generic;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.Entities.Definition;

namespace quiz_project.Interfaces
{
    public interface ICreatorApplicationRepository
    {
        Task<CreatorApplication?> GetByUserIdAsync(int userId);
        Task<List<CreatorApplication>> GetAllAsync();
        Task<List<CreatorApplication>> GetByStatusAsync(ApplicationStatus status);
        Task<CreatorApplication?> GetByIdAsync(int id);
        Task AddAsync(CreatorApplication application);
        Task UpdateAsync(CreatorApplication application);
    }
}
