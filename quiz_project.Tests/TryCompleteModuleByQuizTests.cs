using FluentAssertions;
using Moq;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Tests;

/// <summary>
/// Tests for ChapterService.TryCompleteModuleByQuizAsync —
/// the logic that marks a module complete when its quiz is passed
/// and all chapters are already done.
/// </summary>
public class TryCompleteModuleByQuizTests
{
    private static Module MakeModule(int id = 10, bool requireQuizPass = true, int? quizId = 99) =>
        new() { ModuleId = id, CourseId = 1, Title = "M", Description = "D", RequireQuizPass = requireQuizPass, QuizId = quizId };

    private static Quiz MakeQuiz(int passPerc = 80, int totalScore = 100) =>
        new() { QuizId = 99, Title = "Q", Description = "D", UserId = 1, PassPercentage = passPerc, TotalScore = totalScore };

    private static Chapter MakeChapter(int id, int moduleId = 10) =>
        new() { ChapterId = id, ModuleId = moduleId, Title = "C", Order = id };

    private static ChapterService BuildService(
        Mock<IModuleRepository> moduleRepo,
        Mock<IQuizRepository> quizRepo,
        Mock<IProgressRepository> progressRepo,
        Mock<IChapterRepository> chapterRepo)
    {
        var mapper = new Mock<IChapterMapper>();
        mapper.Setup(m => m.ToViewModel(It.IsAny<Chapter>(), It.IsAny<bool>(), It.IsAny<bool>()))
              .Returns(new ChapterViewModel());

        return new ChapterService(
            chapterRepo.Object,
            moduleRepo.Object,
            new Mock<ICourseRepository>().Object,
            progressRepo.Object,
            mapper.Object,
            new Mock<IAttemptRepository>().Object,
            quizRepo.Object);
    }

    // ── Module not found ─────────────────────────────────────────────────────

    [Fact]
    public async Task TryComplete_ModuleNotFound_DoesNothing()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync((Module?)null);
        var progressRepo = new Mock<IProgressRepository>();

        var svc = BuildService(moduleRepo, new Mock<IQuizRepository>(), progressRepo, new Mock<IChapterRepository>());
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 90, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ── RequireQuizPass = false ──────────────────────────────────────────────

    [Fact]
    public async Task TryComplete_ModuleDoesNotRequireQuizPass_DoesNothing()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync(MakeModule(requireQuizPass: false));
        var progressRepo = new Mock<IProgressRepository>();

        var svc = BuildService(moduleRepo, new Mock<IQuizRepository>(), progressRepo, new Mock<IChapterRepository>());
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 100, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ── Quiz not passed ──────────────────────────────────────────────────────

    [Fact]
    public async Task TryComplete_QuizFailed_DoesNotComplete()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync(MakeModule());

        var quizRepo = new Mock<IQuizRepository>();
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPerc: 80, totalScore: 100));

        var progressRepo = new Mock<IProgressRepository>();

        var svc = BuildService(moduleRepo, quizRepo, progressRepo, new Mock<IChapterRepository>());
        // 60/100 = 60% < 80% → fail
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 60, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ── Not all chapters completed ───────────────────────────────────────────

    [Fact]
    public async Task TryComplete_SomeChaptersIncomplete_DoesNotComplete()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync(MakeModule());

        var quizRepo = new Mock<IQuizRepository>();
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPerc: 80, totalScore: 100));

        var chapters = new List<Chapter> { MakeChapter(1), MakeChapter(2) };
        var chapterRepo = new Mock<IChapterRepository>();
        chapterRepo.Setup(r => r.GetChaptersByModuleIdAsync(10)).ReturnsAsync(chapters);

        var progressRepo = new Mock<IProgressRepository>();
        // Only chapter 1 is completed, chapter 2 is not
        progressRepo.Setup(r => r.GetChapterProgressesForModuleAsync(7, 10))
                    .ReturnsAsync([new UserPartProgress { ChapterId = 1, IsCompleted = true }]);

        var svc = BuildService(moduleRepo, quizRepo, progressRepo, chapterRepo);
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 90, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ── No chapters in module ────────────────────────────────────────────────

    [Fact]
    public async Task TryComplete_NoChaptersInModule_DoesNotComplete()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync(MakeModule());

        var quizRepo = new Mock<IQuizRepository>();
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPerc: 80, totalScore: 100));

        var chapterRepo = new Mock<IChapterRepository>();
        chapterRepo.Setup(r => r.GetChaptersByModuleIdAsync(10)).ReturnsAsync([]);

        var progressRepo = new Mock<IProgressRepository>();

        var svc = BuildService(moduleRepo, quizRepo, progressRepo, chapterRepo);
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 95, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    // ── Happy path: all conditions met ───────────────────────────────────────

    [Fact]
    public async Task TryComplete_AllChaptersComplete_QuizPassed_CompletesModule()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync(MakeModule());

        var quizRepo = new Mock<IQuizRepository>();
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPerc: 80, totalScore: 100));

        var chapters = new List<Chapter> { MakeChapter(1), MakeChapter(2) };
        var chapterRepo = new Mock<IChapterRepository>();
        chapterRepo.Setup(r => r.GetChaptersByModuleIdAsync(10)).ReturnsAsync(chapters);

        var progressRepo = new Mock<IProgressRepository>();
        progressRepo.Setup(r => r.GetChapterProgressesForModuleAsync(7, 10))
                    .ReturnsAsync(
                    [
                        new UserPartProgress { ChapterId = 1, IsCompleted = true },
                        new UserPartProgress { ChapterId = 2, IsCompleted = true }
                    ]);
        progressRepo.Setup(r => r.MarkModuleCompletedAsync(7, 10)).Returns(Task.CompletedTask);

        var svc = BuildService(moduleRepo, quizRepo, progressRepo, chapterRepo);
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 90, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(7, 10), Times.Once);
    }

    // ── PassPercentage = 0 → no threshold ───────────────────────────────────

    [Fact]
    public async Task TryComplete_PassPercentageZero_AnyScoreCountsAsPassed()
    {
        var moduleRepo = new Mock<IModuleRepository>();
        moduleRepo.Setup(r => r.GetModuleByQuizIdAsync(99)).ReturnsAsync(MakeModule());

        var quizRepo = new Mock<IQuizRepository>();
        quizRepo.Setup(r => r.GetQuizByIdAsync(99)).ReturnsAsync(MakeQuiz(passPerc: 0, totalScore: 100));

        var chapters = new List<Chapter> { MakeChapter(1) };
        var chapterRepo = new Mock<IChapterRepository>();
        chapterRepo.Setup(r => r.GetChaptersByModuleIdAsync(10)).ReturnsAsync(chapters);

        var progressRepo = new Mock<IProgressRepository>();
        progressRepo.Setup(r => r.GetChapterProgressesForModuleAsync(7, 10))
                    .ReturnsAsync([new UserPartProgress { ChapterId = 1, IsCompleted = true }]);
        progressRepo.Setup(r => r.MarkModuleCompletedAsync(7, 10)).Returns(Task.CompletedTask);

        var svc = BuildService(moduleRepo, quizRepo, progressRepo, chapterRepo);
        // Even score 1/100 should count as passed when PassPercentage=0
        await svc.TryCompleteModuleByQuizAsync(quizId: 99, userId: 7, score: 1, totalScore: 100);

        progressRepo.Verify(r => r.MarkModuleCompletedAsync(7, 10), Times.Once);
    }
}
