using FluentAssertions;
using Moq;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.Services;

namespace quiz_project.Tests;

public class QuizReminderServiceTests
{
    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 7)]
    [InlineData(2, 14)]
    [InlineData(3, 24)]
    [InlineData(4, 35)]
    [InlineData(5, 46)]
    [InlineData(6, 57)]  
    [InlineData(7, 68)]  
    [InlineData(10, 101)] 
    public void GetIntervalDays_ReturnsCorrectInterval(int index, int expectedDays)
    {
        QuizReminderService.GetIntervalDays(index).Should().Be(expectedDays);
    }

    [Fact]
    public void GetIntervalDays_NegativeIndex_ReturnFirstInterval()
    {
        QuizReminderService.GetIntervalDays(-1).Should().Be(3);
    }


    private static QuizReminderService BuildService(
        Mock<IQuizReminderRepository> repoMock,
        Mock<IAttemptRepository>? attemptMock = null,
        Mock<IQuizRepository>? quizMock = null)
    {
        return new QuizReminderService(
            repoMock.Object,
            (attemptMock ?? new Mock<IAttemptRepository>()).Object,
            (quizMock ?? new Mock<IQuizRepository>()).Object);
    }

    [Fact]
    public async Task OnQuizAttemptFinished_NoReminder_DoesNothing()
    {
        var repo = new Mock<IQuizReminderRepository>();
        repo.Setup(r => r.GetAsync(1, 42)).ReturnsAsync((QuizReminderItem?)null);

        var svc = BuildService(repo);
        await svc.OnQuizAttemptFinishedAsync(userId: 1, quizId: 42, passed: true);

        repo.Verify(r => r.UpdateAsync(It.IsAny<QuizReminderItem>()), Times.Never);
    }

    [Fact]
    public async Task OnQuizAttemptFinished_Passed_AdvancesIndex()
    {
        var reminder = new QuizReminderItem { Id = 1, UserId = 1, QuizId = 42, IntervalIndex = 2, NextReviewDate = DateTime.UtcNow };
        var repo = new Mock<IQuizReminderRepository>();
        repo.Setup(r => r.GetAsync(1, 42)).ReturnsAsync(reminder);

        var svc = BuildService(repo);
        await svc.OnQuizAttemptFinishedAsync(userId: 1, quizId: 42, passed: true);

        reminder.IntervalIndex.Should().Be(3);
        repo.Verify(r => r.UpdateAsync(reminder), Times.Once);
    }

    [Fact]
    public async Task OnQuizAttemptFinished_Failed_ResetsIndexToZero()
    {
        var reminder = new QuizReminderItem { Id = 1, UserId = 1, QuizId = 42, IntervalIndex = 5, NextReviewDate = DateTime.UtcNow };
        var repo = new Mock<IQuizReminderRepository>();
        repo.Setup(r => r.GetAsync(1, 42)).ReturnsAsync(reminder);

        var svc = BuildService(repo);
        await svc.OnQuizAttemptFinishedAsync(userId: 1, quizId: 42, passed: false);

        reminder.IntervalIndex.Should().Be(0);
        repo.Verify(r => r.UpdateAsync(reminder), Times.Once);
    }

    [Fact]
    public async Task OnQuizAttemptFinished_Passed_NextReviewDateIsInFuture()
    {
        var before = DateTime.UtcNow;
        var reminder = new QuizReminderItem { Id = 1, UserId = 1, QuizId = 42, IntervalIndex = 0, NextReviewDate = DateTime.UtcNow };
        var repo = new Mock<IQuizReminderRepository>();
        repo.Setup(r => r.GetAsync(1, 42)).ReturnsAsync(reminder);

        var svc = BuildService(repo);
        await svc.OnQuizAttemptFinishedAsync(userId: 1, quizId: 42, passed: true);

        reminder.NextReviewDate.Should().BeAfter(before.AddDays(6));
    }


    [Fact]
    public async Task AddReminder_WhenAlreadyExists_DoesNotAddAgain()
    {
        var existing = new QuizReminderItem { Id = 5, UserId = 1, QuizId = 42 };
        var repo = new Mock<IQuizReminderRepository>();
        repo.Setup(r => r.GetAsync(1, 42)).ReturnsAsync(existing);

        var svc = BuildService(repo);
        await svc.AddReminderAsync(userId: 1, quizId: 42);

        repo.Verify(r => r.AddAsync(It.IsAny<QuizReminderItem>()), Times.Never);
    }

    [Fact]
    public async Task AddReminder_WhenNew_AddsWithFirstInterval()
    {
        QuizReminderItem? added = null;
        var repo = new Mock<IQuizReminderRepository>();
        repo.Setup(r => r.GetAsync(1, 42)).ReturnsAsync((QuizReminderItem?)null);
        repo.Setup(r => r.AddAsync(It.IsAny<QuizReminderItem>()))
            .Callback<QuizReminderItem>(item => added = item)
            .Returns(Task.CompletedTask);

        var svc = BuildService(repo);
        await svc.AddReminderAsync(userId: 1, quizId: 42);

        added.Should().NotBeNull();
        added!.IntervalIndex.Should().Be(0);
        added.NextReviewDate.Should().BeAfter(DateTime.UtcNow.AddDays(2));
    }
}
