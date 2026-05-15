using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace quiz_project.RuntimeModels
{
    public class QuizMetaData
    {
        public QuizMetaData(int quizId, int userId, int currentPage, int questionCount, List<AnswersForQuestion> userAnswers)
        {
            QuizId = quizId;
            UserId = userId;
            CurrentPage = currentPage;
            QuestionCount = questionCount;
            UserAnswers = userAnswers;
        }
        public int QuizId { get; set; }
        public int UserId { get; set; }
        public int CurrentPage { get; set; }
        public int QuestionCount { get; set; }
        public List<AnswersForQuestion> UserAnswers { get; set; }

        public class AnswersForQuestion
        {
            public int QuestionId { get; set; }
            public List<int> AnswersId { get; set; } = new();
        }
    }
}