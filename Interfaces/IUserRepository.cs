using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.Interfaces
{
    public interface IUserRepository
    {
        public Task<Dictionary<int, int>> GetTotalScoresAsync();
    }
}