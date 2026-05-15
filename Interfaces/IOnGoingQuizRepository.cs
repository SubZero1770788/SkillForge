using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities.Definition;

namespace quiz_project.Interfaces
{
    public interface IOnGoingQuizRepository
    {
        Task<OnGoingQuizState?> GetAsync(int userId, int quizId);
        Task CreateOrUpdateAsync(OnGoingQuizState state);
        Task DeleteAsync(int userId, int quizId);
    }

}