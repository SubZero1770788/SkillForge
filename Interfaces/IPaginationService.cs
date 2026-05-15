using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Infrastructure;


namespace quiz_project.Interfaces
{
    public interface IPaginationService<T> where T : class
    {
        public Task<PaginatedList<T>> GetPagedDataAsync(List<T> query, int page, int pageSize);
    }
}