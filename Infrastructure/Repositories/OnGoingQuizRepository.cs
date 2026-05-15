using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;

namespace quiz_project.Infrastructure.Repositories
{
    public class OnGoingQuizRepository(QuizDb context) : IOnGoingQuizRepository
    {
        public async Task<OnGoingQuizState?> GetAsync(int userId, int quizId)
        {
            return await context.OnGoingQuizStates
                .Include(ogqs => ogqs.Answers)
                .FirstOrDefaultAsync(s => s.UserId == userId && s.QuizId == quizId);
        }

        public async Task CreateOrUpdateAsync(OnGoingQuizState onGoingQuizState)
        {
            var existing = await context.OnGoingQuizStates
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.UserId == onGoingQuizState.UserId && x.QuizId == onGoingQuizState.QuizId);

            if (existing != null)
            {
                existing.CurrentPage++;
                existing.QuestionCount = onGoingQuizState.QuestionCount;

                foreach (var newAnswer in onGoingQuizState.Answers)
                {
                    var existingAnswer = existing.Answers
                        .FirstOrDefault(a => a.QuestionId == newAnswer.QuestionId);

                    if (existingAnswer != null)
                    {
                        existingAnswer.AnswersId = newAnswer.AnswersId;
                    }
                    else
                    {
                        newAnswer.OnGoingQuizStateId = existing.Id;
                        existing.Answers.Add(newAnswer);
                    }
                }

                context.OnGoingQuizStates.Update(existing);
            }
            else
            {
                await context.OnGoingQuizStates.AddAsync(onGoingQuizState);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int userId, int quizId)
        {
            var existing = await GetAsync(userId, quizId);
            if (existing != null)
            {
                context.OnGoingQuizStates.Remove(existing);
                await context.SaveChangesAsync();
            }
        }
    }
}