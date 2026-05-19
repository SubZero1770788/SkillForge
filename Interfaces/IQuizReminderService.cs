using System.Collections.Generic;
using System.Threading.Tasks;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface IQuizReminderService
    {
        /// <summary>Add a quiz to the user's spaced-repetition schedule (first review in 3 days).</summary>
        Task AddReminderAsync(int userId, int quizId);

        /// <summary>Remove a reminder from the schedule.</summary>
        Task RemoveReminderAsync(int reminderId, int userId);

        /// <summary>Called after a quiz attempt is saved. Advances or resets the interval.</summary>
        Task OnQuizAttemptFinishedAsync(int userId, int quizId, bool passed);

        /// <summary>Returns view data for the My Quizzes panel.</summary>
        Task<MyQuizzesViewModel> GetMyQuizzesAsync(int userId);

        /// <summary>Returns count of reminders due today or earlier.</summary>
        Task<int> GetDueCountAsync(int userId);
    }
}
