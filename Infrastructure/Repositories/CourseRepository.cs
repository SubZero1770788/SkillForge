using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using quiz_project.Database;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Entities.Repositories
{
    public class CourseRepository(QuizDb context) : ICourseRepository
    {
        public async Task<IEnumerable<Course>> GetCoursesByUserAsync(int userId)
        {
            var courses = await context.Courses.Where(x => x.UserId == userId)
                                        .Include(c => c.Modules).ToListAsync();
            return courses;
        }
        public async Task<IEnumerable<Course>> GetCoursesAsync()
        {
            var courses = await context.Courses.ToListAsync();
            return courses;
        }
        public async Task<IEnumerable<Course>> GetPublicCourses()
        {
            var courses = await context.Courses.Where(c => c.IsPublic == true).Include(c => c.Modules).ToListAsync();
            return courses;
        }
        public async Task CreateCourseAsync(Course course)
        {
            await context.AddAsync(course);
            await context.SaveChangesAsync();
        }

        //usuwanie kursów ze wszystkimi danymi?
        public async Task DeleteCourseAsync(Course course)
        {
            var oldCourse = await context.Courses.Where(c => c.CourseId == course.CourseId).FirstAsync();
            context.Courses.Remove(oldCourse);
            await context.SaveChangesAsync();
        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            var course = await context.Courses.Where(c => c.CourseId == courseId)
                                        .Include(c => c.Modules).FirstAsync();
            return course;
        }

        // public async Task<List<Question>> GetQuestionsByQuizId(int quizId)
        // {
        //     var questions = await context.Questions.Where(q => q.QuizId == quizId).Include(q => q.Answers).ToListAsync();
        //     return questions;
        // }

        // public async Task<Dictionary<(int QuestionId, int AnswerId), int>> GetAnswerSelectionStatsAsync(int quizId)
        // {
        //     var answerCounts = await context.AnswerSelections
        //         .Where(a => a.QuizAttempt.QuizId == quizId)
        //         .GroupBy(a => new { a.QuestionId, a.AnswerId })
        //         .ToDictionaryAsync(
        //             g => (g.Key.QuestionId, g.Key.AnswerId),
        //             g => g.Count()
        //         );

        //     return answerCounts;
        // }

        // public async Task UpdateQuizAsync(Quiz quiz)
        // {
        //     var oldQuiz = await context.Quizzes
        //         .Include(q => q.Questions)
        //             .ThenInclude(q => q.Answers)
        //         .FirstAsync(q => q.QuizId == quiz.QuizId);

        //     var originalUserId = oldQuiz.UserId;
        //     context.Entry(oldQuiz).CurrentValues.SetValues(quiz);
        //     oldQuiz.UserId = originalUserId;

        //     foreach (var incomingQuestion in quiz.Questions.Where(q => !q.IsDeleted))
        //     {
        //         if (incomingQuestion.QuestionId == 0)
        //         {
        //             incomingQuestion.QuizId = oldQuiz.QuizId;

        //             foreach (var a in incomingQuestion.Answers)
        //             {
        //                 a.AnswerId = 0;
        //                 a.QuestionId = 0;
        //                 a.Question = null;
        //             }

        //             context.Questions.Add(incomingQuestion);
        //         }
        //         else
        //         {
        //             var existingQuestion = oldQuiz.Questions
        //                 .FirstOrDefault(q => q.QuestionId == incomingQuestion.QuestionId);

        //             if (existingQuestion != null)
        //             {
        //                 context.Entry(existingQuestion).CurrentValues.SetValues(incomingQuestion);

        //                 foreach (var incomingAnswer in incomingQuestion.Answers)
        //                 {
        //                     if (incomingAnswer.AnswerId == 0)
        //                     {
        //                         incomingAnswer.QuestionId = existingQuestion.QuestionId;
        //                         incomingAnswer.Question = null;

        //                         context.Answers.Add(incomingAnswer);
        //                     }
        //                     else
        //                     {
        //                         var existingAnswer = existingQuestion.Answers
        //                             .FirstOrDefault(a => a.AnswerId == incomingAnswer.AnswerId);

        //                         if (existingAnswer != null)
        //                         {
        //                             context.Entry(existingAnswer).CurrentValues.SetValues(incomingAnswer);
        //                         }
        //                         else
        //                         {
        //                             incomingAnswer.Question = null;
        //                             if (incomingAnswer.QuestionId == 0)
        //                                 incomingAnswer.QuestionId = existingQuestion.QuestionId;

        //                             context.Attach(incomingAnswer);
        //                             context.Entry(incomingAnswer).State = EntityState.Modified;
        //                         }
        //                     }
        //                 }
        //                 var answersToRemove = existingQuestion.Answers
        //                     .Where(a => !incomingQuestion.Answers.Any(ia => ia.AnswerId == a.AnswerId))
        //                     .ToList();

        //                 if (answersToRemove.Any())
        //                 {
        //                     var toRemoveIds = answersToRemove.Select(a => a.AnswerId).ToHashSet();

        //                     var selections = await context.AnswerSelections
        //                         .Where(x => toRemoveIds.Contains(x.AnswerId))
        //                         .ToListAsync();

        //                     var allStates = await context.AnswerStates
        //                         .Where(x => x.QuestionId == existingQuestion.QuestionId)
        //                         .ToListAsync();

        //                     var statesToRemove = allStates
        //                         .Where(x => x.AnswersId.Any(id => toRemoveIds.Contains(id)))
        //                         .ToList();

        //                     context.AnswerSelections.RemoveRange(selections);
        //                     context.AnswerStates.RemoveRange(statesToRemove);
        //                     context.Answers.RemoveRange(answersToRemove);
        //                 }
        //             }
        //         }
        //     }

        //     // Handle deleted questions
        //     var incomingQuestionIds = quiz.Questions
        //         .Where(q => q.Description.Length > 0)
        //         .Select(q => q.QuestionId)
        //         .ToHashSet();

        //     var questionsToRemove = oldQuiz.Questions
        //         .Where(q => !incomingQuestionIds.Contains(q.QuestionId))
        //         .ToList();

        //     foreach (var question in questionsToRemove)
        //     {
        //         var answerIds = question.Answers.Select(a => a.AnswerId).ToList();

        //         var selections = await context.AnswerSelections
        //             .Where(x => answerIds.Contains(x.AnswerId)).ToListAsync();

        //         var states = (await context.AnswerStates
        //             .Where(x => x.QuestionId == question.QuestionId)
        //             .ToListAsync())
        //             .Where(x => x.AnswersId.Any(id => answerIds.Contains(id)))
        //             .ToList();

        //         context.AnswerSelections.RemoveRange(selections);
        //         context.AnswerStates.RemoveRange(states);
        //         context.Answers.RemoveRange(question.Answers);
        //     }

        //     context.Questions.RemoveRange(questionsToRemove);

        //     // Debug print of all tracked changes
        //     foreach (var entry in context.ChangeTracker.Entries())
        //     {
        //         if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
        //         {
        //             Console.WriteLine($"{entry.Entity.GetType().Name} | State: {entry.State}");

        //             if (entry.Entity is Answer a)
        //                 Console.WriteLine($"  AnswerId: {a.AnswerId}, QuestionId: {a.QuestionId}");
        //             else if (entry.Entity is Question q)
        //                 Console.WriteLine($"  QuestionId: {q.QuestionId}, QuizId: {q.QuizId}");
        //         }
        //     }

        //     await context.SaveChangesAsync();
        // }

        public Task UpdateCourseAsync(Course course)
        {
            throw new NotImplementedException();
        }

        public Task<List<Module>> GetModulesByCourseId(int courseId)
        {
            throw new NotImplementedException();
        }
    }
}