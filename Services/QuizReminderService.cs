using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class QuizReminderService(
        IQuizReminderRepository reminderRepository,
        IAttemptRepository attemptRepository,
        IQuizRepository quizRepository) : IQuizReminderService
    {
        // Spaced-repetition schedule (days between reviews):
        //   index 0 → 3 d, 1 → 7 d, 2 → 14 d, 3 → 24 d, 4 → 35 d, 5 → 46 d
        //   index 6+ → 46 + (index−5)×11 d  (57, 68, 79, …)
        private static readonly int[] BaseIntervals = { 3, 7, 14, 24, 35, 46 };

        public static int GetIntervalDays(int index)
        {
            if (index < 0) return BaseIntervals[0];
            if (index < BaseIntervals.Length) return BaseIntervals[index];
            return 46 + (index - 5) * 11;
        }

        public async Task AddReminderAsync(int userId, int quizId)
        {
            var existing = await reminderRepository.GetAsync(userId, quizId);
            if (existing is not null) return; // already tracked

            var item = new QuizReminderItem
            {
                UserId = userId,
                QuizId = quizId,
                IntervalIndex = 0,
                NextReviewDate = DateTime.UtcNow.AddDays(GetIntervalDays(0)),
                AddedAt = DateTime.UtcNow
            };
            await reminderRepository.AddAsync(item);
        }

        public async Task RemoveReminderAsync(int reminderId, int userId)
            => await reminderRepository.DeleteAsync(reminderId, userId);

        public async Task OnQuizAttemptFinishedAsync(int userId, int quizId, bool passed)
        {
            var reminder = await reminderRepository.GetAsync(userId, quizId);
            if (reminder is null) return; // user hasn't added this to reminders

            if (passed)
            {
                // Advance to the next interval
                reminder.IntervalIndex++;
            }
            else
            {
                // Failed — reset to first interval
                reminder.IntervalIndex = 0;
            }

            reminder.NextReviewDate = DateTime.UtcNow.AddDays(GetIntervalDays(reminder.IntervalIndex));
            await reminderRepository.UpdateAsync(reminder);
        }

        public async Task<MyQuizzesViewModel> GetMyQuizzesAsync(int userId)
        {
            // Load all attempts for this user (best per quiz)
            var allAttempts = await attemptRepository.GetAllBestAttemptsForUserAsync(userId);

            // Load reminder map
            var reminders = await reminderRepository.GetByUserAsync(userId);
            var reminderMap = reminders.ToDictionary(r => r.QuizId);

            // Build quiz items, loading quiz definitions for those missing nav data
            var quizIds = allAttempts.Select(a => a.QuizId).ToList();
            var quizDefs = await quizRepository.GetQuizzesByIdsAsync(quizIds);

            var items = allAttempts.Select(attempt =>
            {
                quizDefs.TryGetValue(attempt.QuizId, out var quiz);
                var title = quiz?.Title ?? attempt.Quiz?.Title ?? $"Quiz #{attempt.QuizId}";
                var totalScore = quiz?.TotalScore ?? attempt.Quiz?.TotalScore ?? 0;
                var passPerc = quiz?.PassPercentage ?? attempt.Quiz?.PassPercentage ?? 0;

                var passed = passPerc == 0
                    || (totalScore > 0 && (double)attempt.Score / totalScore * 100 >= passPerc);

                reminderMap.TryGetValue(attempt.QuizId, out var reminder);
                var idx = reminder?.IntervalIndex ?? 0;

                return new AttemptedQuizItem
                {
                    QuizId = attempt.QuizId,
                    Title = title,
                    BestScore = attempt.Score,
                    TotalScore = totalScore,
                    Passed = passed,
                    HasReminder = reminder is not null,
                    ReminderId = reminder?.Id,
                    NextReviewDate = reminder?.NextReviewDate,
                    IntervalIndex = idx,
                    CurrentIntervalDays = GetIntervalDays(idx)
                };
            })
            .OrderBy(q => q.IsDue ? 0 : 1)   // due items first
            .ThenBy(q => q.NextReviewDate ?? DateTime.MaxValue)
            .ToList();

            return new MyQuizzesViewModel { Quizzes = items };
        }

        public async Task<int> GetDueCountAsync(int userId)
            => await reminderRepository.GetDueCountAsync(userId);
    }
}
