using System.Collections.Generic;
using System.Threading.Tasks;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IQuizReminderService
    {
        Task AddReminderAsync(int userId, int quizId);
        Task RemoveReminderAsync(int reminderId, int userId);
        Task OnQuizAttemptFinishedAsync(int userId, int quizId, bool passed);
        Task<MyQuizzesViewModel> GetMyQuizzesAsync(int userId);
        Task<int> GetDueCountAsync(int userId);
    }
}
