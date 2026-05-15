using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Infrastructure;
using quiz_project.Interfaces;

namespace quiz_project.Services
{
    public class PaginationService<T> : IPaginationService<T> where T : class
    {
        public Task<PaginatedList<T>> GetPagedDataAsync(List<T> query, int page, int pageSize)
        {
            return PaginatedList<T>.CreateAsync(query, page, pageSize);
        }
    }
}