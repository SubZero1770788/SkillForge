using FluentAssertions;
using Moq;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.Services;
using quiz_project.ViewModels;

namespace quiz_project.Tests;

public class AccessValidationServiceTests
{
    private static AccessValidationService BuildService() =>
        new(new Mock<IQuizRepository>().Object,
            new Mock<ICourseRepository>().Object,
            new Mock<IModuleRepository>().Object);

    private static QuestionViewModel ClosedQuestion(bool hasCorrect = true, int answerCount = 2) =>
        new()
        {
            Type = QuestionType.MultipleChoice,
            Description = "Test question",
            Answers = Enumerable.Range(1, answerCount)
                .Select((_, i) => new AnswerViewModel { IsCorrect = i == 0 && hasCorrect })
                .ToList()
        };

    private static QuestionViewModel OpenQuestion(QuestionType type = QuestionType.FillInTheBlank) =>
        new() { Type = type, Description = "Describe it" };

    // ── open questions are always valid ─────────────────────────────────────

    [Theory]
    [InlineData(QuestionType.FillInTheBlank)]
    [InlineData(QuestionType.OpenWithImage)]
    public void OpenQuestions_NoAnswers_ProduceNoErrors(QuestionType type)
    {
        var svc = BuildService();
        var vm = new QuizViewModel { Questions = [OpenQuestion(type)] };

        svc.EachQuestionHasAnswer(vm).Should().BeEmpty();
    }

    // ── closed questions require ≥2 answers ─────────────────────────────────

    [Fact]
    public void ClosedQuestion_SingleAnswer_ProducesError()
    {
        var svc = BuildService();
        var vm = new QuizViewModel { Questions = [ClosedQuestion(answerCount: 1)] };

        var errors = svc.EachQuestionHasAnswer(vm);
        errors.Should().ContainSingle()
              .Which.Should().Contain("at least 2 answers");
    }

    // ── closed questions require at least one correct answer ─────────────────

    [Fact]
    public void ClosedQuestion_NoCorrectAnswer_ProducesError()
    {
        var svc = BuildService();
        var vm = new QuizViewModel { Questions = [ClosedQuestion(hasCorrect: false, answerCount: 2)] };

        var errors = svc.EachQuestionHasAnswer(vm);
        errors.Should().ContainSingle()
              .Which.Should().Contain("at least one answer marked as correct");
    }

    // ── valid closed question ────────────────────────────────────────────────

    [Fact]
    public void ValidClosedQuestion_ProducesNoErrors()
    {
        var svc = BuildService();
        var vm = new QuizViewModel { Questions = [ClosedQuestion()] };

        svc.EachQuestionHasAnswer(vm).Should().BeEmpty();
    }

    // ── error references correct question number ─────────────────────────────

    [Fact]
    public void ErrorMessage_ContainsQuestionNumber()
    {
        var svc = BuildService();
        var vm = new QuizViewModel
        {
            Questions = [ClosedQuestion(), ClosedQuestion(hasCorrect: false)]
        };

        var errors = svc.EachQuestionHasAnswer(vm);
        errors.Should().ContainSingle()
              .Which.Should().StartWith("Question 2:");
    }

    // ── mixed quiz (open + closed) ───────────────────────────────────────────

    [Fact]
    public void MixedQuestions_ValidAll_ProducesNoErrors()
    {
        var svc = BuildService();
        var vm = new QuizViewModel
        {
            Questions = [OpenQuestion(), ClosedQuestion(), OpenQuestion(QuestionType.OpenWithImage)]
        };

        svc.EachQuestionHasAnswer(vm).Should().BeEmpty();
    }

    [Fact]
    public void EmptyQuiz_ProducesNoErrors()
    {
        var svc = BuildService();
        var vm = new QuizViewModel { Questions = [] };

        svc.EachQuestionHasAnswer(vm).Should().BeEmpty();
    }
}
