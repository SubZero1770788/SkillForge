using FluentAssertions;
using Moq;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Tests;

public class ChapterServiceTests
{
    // ── Helpers 

    private static Chapter MakeChapter(int id = 1, bool requireQuizPass = false, int? quizId = null) =>
        new() { ChapterId = id, ModuleId = 10, Order = 1, Title = "Chapter", RequireQuizPass = requireQuizPass, QuizId = quizId };

    private static Quiz MakeQuiz(int passPercentage = 80, int totalScore = 100) =>
        new() { QuizId = 99, Title = "Q", Description = "D", UserId = 1, PassPercentage = passPercentage, TotalScore = totalScore };

    private static QuizAttempt MakeAttempt(int score) =>
        new() { QuizAttemptId = 1, QuizId = 99, UserId = 7, Score = score };

    private static ChapterService BuildService(
        Mock<IChapterRepository> chapterRepo,
        Mock<IAttemptRepository> attemptRepo,
        Mock<IQuizRepository> quizRepo,
        Mock<IProgressRepository> progressRepo,
        Mock<IModuleRepository>? moduleRepo = null,
        Mock<ICourseRepository>? courseRepo = null)
    {
        var chapterMapper = new Mock<IChapterMapper>();
        chapterMapper.Setup(m => m.ToViewModel(It.IsAny<Chapter>(), It.IsAny<bool>(), It.IsAny<bool>()))
                     .Returns(new ChapterViewModel());

        var fileStorage = new Mock<IFileStorageService>();
        fileStorage.Setup(f => f.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Default: non-sequential course so sequential guard is never triggered
        if (courseRepo is null)
        {
            courseRepo = new Mock<ICourseRepository>();
            courseRepo.Setup(r => r.GetCourseByIdAsync(It.IsAny<int>()))
                      .ReturnsAsync(new Course { CourseId = 1, IsSequential = false, Title = "C", Description = "D", UserId = 1 });
        }

        return new ChapterService(
            chapterRepo.Object,
            (moduleRepo ?? new Mock<IModuleRepository>()).Object,
            courseRepo.Object,
            progressRepo.Object,
            chapterMapper.Object,
            attemptRepo.Object,
            quizRepo.Object,
            fileStorage.Object);
    }

    // ── MarkAsCompletedAsync-no quiz gate 

    [Fact]
    public async Task MarkAsCompleted_NoRequireQuizPass_MarksChapterCompleted()
    {
        var chapter = MakeChapter(requireQuizPass: false);
        var chapterRepo = new Mock<IChapterRepository>();
        var progressRepo = new Mock<IProgressRepository>();
        var moduleRepo = new Mock<IModuleRepository>();

        chapterRepo.Setup(r => r.GetChapterByIdAsync(1)).ReturnsAsync(chapter);
        chapterRepo.Setup(r => r.GetChaptersByModuleIdAsync(10)).ReturnsAsync([chapter]);
        progressRepo.Setup(r => r.MarkChapterCompletedAsync(7, 1)).Returns(Task.CompletedTask);
        progressRepo.Setup(r => r.GetChapterProgressesForModuleAsync(7, 10))
                    .ReturnsAsync([new UserPartProgress { ChapterId = 1, IsCompleted = true }]);
        moduleRepo.Setup(r => r.GetModuleByIdAsync(10))
                  .ReturnsAsync(new Module { ModuleId = 10, CourseId = 1, Title = "M", Description = "D" });
        progressRepo.Setup(r => r.MarkModuleCompletedAsync(7, 10)).Returns(Task.CompletedTask);

        var svc = BuildService(chapterRepo, new Mock<IAttemptRepository>(), new Mock<IQuizRepository>(), progressRepo, moduleRepo);
        await svc.MarkAsCompletedAsync(chapterId: 1, userId: 7);

        progressRepo.Verify(r => r.MarkChapterCompletedAsync(7, 1), Times.Once);
    }

    // ── MarkAsCompletedAsync-quiz not passed → blocked

    [Fact]
    public async Task MarkAsCompleted_RequireQuizPass_QuizFailed_DoesNotMark()
    {
        var chapter = MakeChapter(requireQuizPass: true, quizId: 99);
        var chapterRepo = new Mock<IChapterRepository>();
        var attemptRepo = new Mock<IAttemptRepository>();
        var quizRepo = new Mock<IQuizRepository>();
        var progressRepo = new Mock<IProgressRepository>();

        chapterRepo.Setup(r => r.GetChapterByIdAsync(1)).ReturnsAsync(chapter);
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPercentage: 80));
        attemptRepo.Setup(r => r.GetTopUserAttemptAsync(7, 99)).ReturnsAsync(MakeAttempt(score: 50));

        var svc = BuildService(chapterRepo, attemptRepo, quizRepo, progressRepo);
        await svc.MarkAsCompletedAsync(chapterId: 1, userId: 7);

        progressRepo.Verify(r => r.MarkChapterCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task MarkAsCompleted_ChapterHasQuiz_BlocksManualCompletion_EvenWhenQuizPassed()
    {
        var chapter = MakeChapter(requireQuizPass: true, quizId: 99);
        var chapterRepo = new Mock<IChapterRepository>();
        var progressRepo = new Mock<IProgressRepository>();

        chapterRepo.Setup(r => r.GetChapterByIdAsync(1)).ReturnsAsync(chapter);

        var svc = BuildService(chapterRepo, new Mock<IAttemptRepository>(), new Mock<IQuizRepository>(), progressRepo);
        await svc.MarkAsCompletedAsync(chapterId: 1, userId: 7);

        progressRepo.Verify(r => r.MarkChapterCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task MarkAsCompleted_ChapterHasQuiz_BlocksManualCompletion_WhenPassPercentageZero()
    {
        var chapter = MakeChapter(requireQuizPass: true, quizId: 99);
        var chapterRepo = new Mock<IChapterRepository>();
        var progressRepo = new Mock<IProgressRepository>();

        chapterRepo.Setup(r => r.GetChapterByIdAsync(1)).ReturnsAsync(chapter);

        var svc = BuildService(chapterRepo, new Mock<IAttemptRepository>(), new Mock<IQuizRepository>(), progressRepo);
        await svc.MarkAsCompletedAsync(chapterId: 1, userId: 7);

        progressRepo.Verify(r => r.MarkChapterCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }


    [Fact]
    public async Task MarkAsCompleted_RequireQuizPass_NoAttempt_DoesNotMark()
    {
        var chapter = MakeChapter(requireQuizPass: true, quizId: 99);
        var chapterRepo = new Mock<IChapterRepository>();
        var attemptRepo = new Mock<IAttemptRepository>();
        var quizRepo = new Mock<IQuizRepository>();
        var progressRepo = new Mock<IProgressRepository>();

        chapterRepo.Setup(r => r.GetChapterByIdAsync(1)).ReturnsAsync(chapter);
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPercentage: 80));
        attemptRepo.Setup(r => r.GetTopUserAttemptAsync(7, 99)).ReturnsAsync((QuizAttempt?)null);

        var svc = BuildService(chapterRepo, attemptRepo, quizRepo, progressRepo);
        await svc.MarkAsCompletedAsync(chapterId: 1, userId: 7);

        progressRepo.Verify(r => r.MarkChapterCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}
