using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using quiz_project.Entities;

namespace quiz_project.Interfaces
{
    public interface IQuizReminderRepository
    {
        Task<QuizReminderItem?> GetAsync(int userId, int quizId);
        Task<List<QuizReminderItem>> GetByUserAsync(int userId);
        Task<int> GetDueCountAsync(int userId);
        Task AddAsync(QuizReminderItem item);
        Task UpdateAsync(QuizReminderItem item);
        Task DeleteAsync(int id, int userId);
    }
}
