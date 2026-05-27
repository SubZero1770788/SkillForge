using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Infrastructure;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Tests;

public class QuizScoringTests
{

    private static (QuizGameService svc, Mock<IAttemptRepository> attemptRepo, Mock<IOnGoingQuizRepository> ongoingRepo, Mock<IQuizRepository> quizRepo)
        BuildSvc()
    {
        var store = new Mock<IUserStore<User>>();
        var userManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

        var attemptRepo = new Mock<IAttemptRepository>();
        attemptRepo.Setup(r => r.CreateAsync(It.IsAny<QuizAttempt>())).Returns(Task.CompletedTask);

        var ongoingRepo = new Mock<IOnGoingQuizRepository>();
        ongoingRepo.Setup(r => r.DeleteAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        ongoingRepo.Setup(r => r.CreateOrUpdateAsync(It.IsAny<OnGoingQuizState>())).Returns(Task.CompletedTask);

        var quizRepo = new Mock<IQuizRepository>();

        var reminderSvc = new Mock<IQuizReminderService>();
        reminderSvc.Setup(r => r.OnQuizAttemptFinishedAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()))
                   .Returns(Task.CompletedTask);

        var chapterSvc = new Mock<IChapterService>();
        chapterSvc.Setup(c => c.TryCompleteModuleByQuizAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                  .Returns(Task.CompletedTask);

        var svc = new QuizGameService(
            quizRepo.Object,
            userManager.Object,
            new Mock<IQuizMapper>().Object,
            ongoingRepo.Object,
            attemptRepo.Object,
            new Mock<IAccessValidationService>().Object,
            new Mock<IQuizQueryService>().Object,
            new PaginationService<Question>(),
            chapterSvc.Object,
            reminderSvc.Object);

        return (svc, attemptRepo, ongoingRepo, quizRepo);
    }
    private static OnGoingQuizState MakeFinishedState(int quizId, int userId,
        List<AnswerState> answers, List<int> questionIds)
    {
        return new OnGoingQuizState
        {
            QuizId = quizId,
            UserId = userId,
            CurrentPage = 2,   
            QuestionCount = 5,
            Questions = questionIds
                .Select((id, i) => new OnGoingQuizQuestion { QuestionId = id, Order = i })
                .ToList(),
            Answers = answers
        };
    }

    private static Question ClosedQuestion(int id, int score, List<(int answerId, bool correct)> answers,
        QuestionType type = QuestionType.MultipleChoice) =>
        new()
        {
            QuestionId = id,
            Description = "Q",
            QuestionScore = score,
            Type = type,
            Answers = answers.Select(a => new Answer
            {
                AnswerId = a.answerId,
                Description = "A",
                IsCorrect = a.correct
            }).ToList()
        };

    private static Question OpenQuestion(int id, int score, string keywords,
        QuestionType type = QuestionType.FillInTheBlank, GradingMethod? grading = null) =>
        new()
        {
            QuestionId = id,
            Description = "Q",
            QuestionScore = score,
            Type = type,
            Grading = grading,
            Keywords = keywords
        };

    private static Quiz StandaloneQuiz(int quizId, int totalScore, int passPerc = 0) =>
        new() { QuizId = quizId, Title = "T", Description = "D", UserId = 1, TotalScore = totalScore, PassPercentage = passPerc };


    [Fact]
    public async Task MultipleChoice_AllCorrectAnswers_GetsFullScore()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = ClosedQuestion(1, 10,
            [(1, true), (2, false), (3, false)]);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, AnswersId = [1] }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 10));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 10)), Times.Once);
    }

    [Fact]
    public async Task MultipleChoice_WrongAnswer_GetsZero()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = ClosedQuestion(1, 10,
            [(1, true), (2, false)]);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, AnswersId = [2] }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 10));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 0)), Times.Once);
    }

    [Fact]
    public async Task MultipleChoice_PartialCorrect_GetsZero()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = ClosedQuestion(1, 10,
            [(1, true), (2, true), (3, false)]);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, AnswersId = [1] }],  
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 10));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 0)), Times.Once);
    }

    [Fact]
    public async Task MultipleChoice_NoAnswerSelected_GetsZero()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = ClosedQuestion(1, 10, [(1, true), (2, false)]);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, AnswersId = [] }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 10));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 0)), Times.Once);
    }


    [Fact]
    public async Task FillInTheBlank_MatchingKeyword_GetsFullScore()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = OpenQuestion(1, 15, "photosynthesis, chlorophyll");

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, OpenText = "photosynthesis" }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 15));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 15)), Times.Once);
    }

    [Fact]
    public async Task FillInTheBlank_CaseInsensitive_GetsFullScore()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = OpenQuestion(1, 15, "photosynthesis");

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, OpenText = "PHOTOSYNTHESIS" }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 15));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 15)), Times.Once);
    }

    [Fact]
    public async Task FillInTheBlank_NoMatchingKeyword_GetsZero()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = OpenQuestion(1, 15, "photosynthesis");

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, OpenText = "mitosis" }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 15));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 0)), Times.Once);
    }

    [Fact]
    public async Task FillInTheBlank_EmptyAnswer_GetsZero()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = OpenQuestion(1, 10, "answer");

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, OpenText = "" }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 10));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 0)), Times.Once);
    }


    [Fact]
    public async Task OpenWithImage_KeywordGrading_Match_GetsFullScore()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = OpenQuestion(1, 20, "newton, gravity",
            QuestionType.OpenWithImage, GradingMethod.Keywords);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, OpenText = "Newton" }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 20));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 20)), Times.Once);
    }

    [Fact]
    public async Task OpenWithImage_ManualGrading_NotAutoGraded_ScoreZero()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = OpenQuestion(1, 20, null,
            QuestionType.OpenWithImage, GradingMethod.Manual);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, OpenText = "some answer" }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 20));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a =>
                a.Score == 0 &&
                a.OpenAnswerRecords.Any(rec => !rec.IsGraded))),
            Times.Once);
    }


    [Fact]
    public async Task MixedQuestions_ScoreSumsCorrectly()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var q1 = ClosedQuestion(1, 10, [(1, true), (2, false)]);             
        var q2 = ClosedQuestion(2, 10, [(3, true), (4, false)]);            
        var q3 = OpenQuestion(3, 20, "key");                                 

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers:
            [
                new AnswerState { QuestionId = 1, AnswersId = [1] },
                new AnswerState { QuestionId = 2, AnswersId = [4] },
                new AnswerState { QuestionId = 3, OpenText = "key" }
            ],
            questionIds: [1, 2, 3]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([q1, q2, q3]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 40));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r =>
            r.CreateAsync(It.Is<QuizAttempt>(a => a.Score == 30)), Times.Once);
    }

    [Fact]
    public async Task ClosedQuestion_AnswerSelections_StoredWithCorrectness()
    {
        var (svc, attemptRepo, ongoingRepo, quizRepo) = BuildSvc();

        var question = ClosedQuestion(1, 10, [(1, true), (2, false)]);

        var state = MakeFinishedState(quizId: 1, userId: 7,
            answers: [new AnswerState { QuestionId = 1, AnswersId = [1, 2] }],
            questionIds: [1]);

        ongoingRepo.Setup(r => r.GetAsync(7, 1)).ReturnsAsync(state);
        quizRepo.Setup(r => r.GetQuestionsByQuizId(1)).ReturnsAsync([question]);
        quizRepo.Setup(r => r.GetQuizByIdAsync(1)).ReturnsAsync(StandaloneQuiz(1, 10));

        await svc.SubmitPlayAsync(new GameViewModel { QuizId = 1 }, userId: 7);

        attemptRepo.Verify(r => r.CreateAsync(It.Is<QuizAttempt>(a =>
            a.AnswerSelections.Any(sel => sel.AnswerId == 1 && sel.IsCorrect) &&
            a.AnswerSelections.Any(sel => sel.AnswerId == 2 && !sel.IsCorrect))),
            Times.Once);
    }
}
