using FluentAssertions;
using Moq;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Tests;

public class QuizServiceTests
{
    private static QuizService BuildService(Mock<IQuizRepository>? repoMock = null)
    {
        repoMock ??= new Mock<IQuizRepository>();
        repoMock.Setup(r => r.CreateQuizAsync(It.IsAny<Quiz>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.UpdateQuizAsync(It.IsAny<Quiz>())).Returns(Task.CompletedTask);

        var mapperMock = new Mock<IQuizMapper>();
        mapperMock.Setup(m => m.ToEntity(It.IsAny<QuizViewModel>(), It.IsAny<int>()))
                  .Returns(new Quiz { Title = "T", Description = "D", UserId = 1 });

        var validationMock = new Mock<IAccessValidationService>();
        validationMock.Setup(v => v.EachQuestionHasAnswer(It.IsAny<QuizViewModel>()))
                      .Returns(new List<string>());

        // No attempts by default — quiz is unlocked for editing
        var attemptRepo = new Mock<IAttemptRepository>();
        attemptRepo.Setup(r => r.GetAllAttemptsAsync(It.IsAny<int>()))
                   .ReturnsAsync(new List<QuizAttempt>());

        return new QuizService(repoMock.Object, mapperMock.Object, validationMock.Object, attemptRepo.Object);
    }

    private static QuizViewModel PublicQuizVm(int questionCount = 5, int totalScore = 50) =>
        new()
        {
            Title = "Test",
            Description = "Desc",
            IsPublic = true,
            Questions = Enumerable.Range(1, questionCount)
                .Select(_ => new QuestionViewModel
                {
                    Description = "Q",
                    QuestionScore = totalScore / questionCount,
                    Answers = [new AnswerViewModel { IsCorrect = true }, new AnswerViewModel()]
                })
                .ToList()
        };

    // ── CreateAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task Create_ValidPublicQuiz_Succeeds()
    {
        var svc = BuildService();
        var (success, error) = await svc.CreateAsync(PublicQuizVm(), userId: 1);

        success.Should().BeTrue();
        error.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_PublicQuiz_TooFewQuestions_Fails()
    {
        var svc = BuildService();
        var vm = PublicQuizVm(questionCount: 4, totalScore: 60);

        var (success, error) = await svc.CreateAsync(vm, userId: 1);

        success.Should().BeFalse();
        error.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_PublicQuiz_TooLowScore_Fails()
    {
        var svc = BuildService();
        var vm = PublicQuizVm(questionCount: 5, totalScore: 40);

        var (success, error) = await svc.CreateAsync(vm, userId: 1);

        success.Should().BeFalse();
        error.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Create_PrivateQuiz_FewQuestionsAllowed()
    {
        var svc = BuildService();
        var vm = new QuizViewModel
        {
            Title = "T", Description = "D", IsPublic = false,
            Questions = [new QuestionViewModel
            {
                Description = "Q", QuestionScore = 10,
                Answers = [new AnswerViewModel { IsCorrect = true }, new AnswerViewModel()]
            }]
        };

        var (success, _) = await svc.CreateAsync(vm, userId: 1);
        success.Should().BeTrue();
    }

    // ── PostEditAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task PostEdit_ValidPublicQuiz_Succeeds()
    {
        var svc = BuildService();
        var (success, errors) = await svc.PostEditAsync(PublicQuizVm(), userId: 1);

        success.Should().BeTrue();
        errors.Should().BeEmpty();
    }

    [Fact]
    public async Task PostEdit_PublicQuiz_BelowMinimums_Fails()
    {
        var svc = BuildService();
        var vm = PublicQuizVm(questionCount: 3, totalScore: 20);

        var (success, errors) = await svc.PostEditAsync(vm, userId: 1);

        success.Should().BeFalse();
        errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task PostEdit_ValidationErrors_ReturnedAsIs()
    {
        var repoMock = new Mock<IQuizRepository>();
        var mapperMock = new Mock<IQuizMapper>();
        var validationMock = new Mock<IAccessValidationService>();
        validationMock.Setup(v => v.EachQuestionHasAnswer(It.IsAny<QuizViewModel>()))
                      .Returns(["Question 1: must have at least one answer marked as correct."]);
        var attemptRepo = new Mock<IAttemptRepository>();
        attemptRepo.Setup(r => r.GetAllAttemptsAsync(It.IsAny<int>())).ReturnsAsync(new List<QuizAttempt>());

        var svc = new QuizService(repoMock.Object, mapperMock.Object, validationMock.Object, attemptRepo.Object);
        var (success, errors) = await svc.PostEditAsync(new QuizViewModel { Questions = [] }, userId: 1);

        success.Should().BeFalse();
        errors.Should().ContainSingle();
    }

    // ── Attempt lock ─────────────────────────────────────────────────────────

    [Fact]
    public async Task PostEdit_QuizHasAttempts_BlocksEdit()
    {
        var repoMock = new Mock<IQuizRepository>();
        var mapperMock = new Mock<IQuizMapper>();
        var validationMock = new Mock<IAccessValidationService>();
        validationMock.Setup(v => v.EachQuestionHasAnswer(It.IsAny<QuizViewModel>()))
                      .Returns(new List<string>());

        var attemptRepo = new Mock<IAttemptRepository>();
        attemptRepo.Setup(r => r.GetAllAttemptsAsync(It.IsAny<int>()))
                   .ReturnsAsync(new List<QuizAttempt> { new() { QuizId = 1, UserId = 7 } });

        var svc = new QuizService(repoMock.Object, mapperMock.Object, validationMock.Object, attemptRepo.Object);
        var (success, errors) = await svc.PostEditAsync(PublicQuizVm(), userId: 1);

        success.Should().BeFalse();
        errors.Should().ContainSingle().Which.Should().Contain("recorded attempts");
        repoMock.Verify(r => r.UpdateQuizAsync(It.IsAny<Quiz>()), Times.Never);
    }

    [Fact]
    public async Task HasAttempts_ReturnsTrue_WhenAttemptsExist()
    {
        var repoMock = new Mock<IQuizRepository>();
        var attemptRepo = new Mock<IAttemptRepository>();
        attemptRepo.Setup(r => r.GetAllAttemptsAsync(5))
                   .ReturnsAsync(new List<QuizAttempt> { new() { QuizId = 5, UserId = 7 } });

        var svc = new QuizService(repoMock.Object, new Mock<IQuizMapper>().Object,
            new Mock<IAccessValidationService>().Object, attemptRepo.Object);

        (await svc.HasAttemptsAsync(5)).Should().BeTrue();
    }

    [Fact]
    public async Task HasAttempts_ReturnsFalse_WhenNoAttempts()
    {
        var repoMock = new Mock<IQuizRepository>();
        var attemptRepo = new Mock<IAttemptRepository>();
        attemptRepo.Setup(r => r.GetAllAttemptsAsync(5)).ReturnsAsync(new List<QuizAttempt>());

        var svc = new QuizService(repoMock.Object, new Mock<IQuizMapper>().Object,
            new Mock<IAccessValidationService>().Object, attemptRepo.Object);

        (await svc.HasAttemptsAsync(5)).Should().BeFalse();
    }
}
