using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class QuizGameController(
        IAccessValidationService accessValidationService,
        UserManager<User> userManager,
        IQuizGameService quizGameService,
        IQuizQueryService quizQueryService,
        IChapterRepository chapterRepository,
        IAttemptRepository attemptRepository) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(int QuizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            if (!await CanUserAccessQuizAsync(QuizId, user))
                return RedirectToAction("Index");

            await quizGameService.StartQuizAsync(QuizId, user.Id);

            return RedirectToAction("Play", new { QuizId });
        }

        [HttpGet, ActionName("Play")]
        public async Task<IActionResult> Play(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            if (!await CanUserAccessQuizAsync(quizId, user))
                return RedirectToAction("Index");

            var (success, gameViewModel) = await quizGameService.GetPlayAsync(quizId, user.Id);

            if (!success || gameViewModel is null)
                return View("Empty");

            return View(gameViewModel);
        }

        [HttpPost, ActionName("Play")]
        public async Task<IActionResult> Play(GameViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            if (!await CanUserAccessQuizAsync(model.QuizId, user))
                return RedirectToAction("Index");

            var (finished, gameViewModel) = await quizGameService.SubmitPlayAsync(model, user.Id);

            if (finished)
                return RedirectToAction("Summary", new { QuizId = model.QuizId });

            if (gameViewModel is null)
                return View("Empty");

            return View(gameViewModel);
        }

        [HttpGet, ActionName("Summary")]
        public async Task<IActionResult> QuizAttemptSummary(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            if (!await CanUserAccessQuizAsync(quizId, user))
                return User.IsInRole("Creator")
                    ? RedirectToAction("Index", "Quiz")
                    : RedirectToAction("MyCourses", "Course");

            var (success, quizSummaryViewModel) = await quizGameService.AttemptSummary(quizId, user);

            if (!success)
                return User.IsInRole("Creator")
                    ? RedirectToAction("Index", "Quiz")
                    : RedirectToAction("MyCourses", "Course");

            var chapter = await chapterRepository.GetChapterByQuizIdForUserAsync(quizId, user.Id);
            if (chapter is not null)
                ViewBag.ReturnChapterId = chapter.ChapterId;

            return View(quizSummaryViewModel);
        }

        [HttpGet, ActionName("ReviewAttempt")]
        [Authorize]
        public async Task<IActionResult> ReviewAttemptAsync(int attemptId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var attempt = await attemptRepository.GetAttemptFullDetailAsync(attemptId);
            if (attempt is null) return NotFound();

            if (attempt.UserId != user.Id && !User.IsInRole("Creator") && !User.IsInRole("Admin"))
                return Forbid();

            var quiz = attempt.Quiz;

            var questions = quiz.Questions.Select(q =>
            {
                var openRecord = attempt.OpenAnswerRecords.FirstOrDefault(r => r.QuestionId == q.QuestionId);

                int earned = 0;
                if (q.Type == QuestionType.MultipleChoice || q.Type == QuestionType.SingleChoice)
                {
                    var correctIds = q.Answers.Where(a => a.IsCorrect).Select(a => a.AnswerId).OrderBy(x => x).ToList();
                    var userIds = attempt.AnswerSelections.Where(s => s.QuestionId == q.QuestionId).Select(s => s.AnswerId).OrderBy(x => x).ToList();
                    earned = correctIds.SequenceEqual(userIds) ? q.QuestionScore : 0;
                }
                else if (openRecord != null)
                {
                    earned = openRecord.ManualScore ?? 0;
                }

                return new AttemptQuestionDetail
                {
                    Description = q.Description,
                    Type = q.Type,
                    ImagePath = q.ImagePath,
                    MaxScore = q.QuestionScore,
                    EarnedScore = earned,
                    Keywords = q.Keywords,
                    OpenText = openRecord?.OpenText,
                    IsGraded = openRecord?.IsGraded ?? true,
                    ManualScore = openRecord?.ManualScore,
                    Options = q.Answers.Select(a => new AttemptAnswerOption
                    {
                        Description = a.Description,
                        WasSelected = attempt.AnswerSelections.Any(s => s.QuestionId == q.QuestionId && s.AnswerId == a.AnswerId),
                        IsCorrect = a.IsCorrect
                    }).ToList()
                };
            }).ToList();

            var vm = new AttemptDetailViewModel
            {
                AttemptId = attempt.QuizAttemptId,
                UserName = attempt.User?.UserName ?? "Unknown",
                QuizTitle = quiz.Title,
                EarnedScore = attempt.Score,
                TotalScore = quiz.TotalScore,
                UserId = attempt.UserId,
                CourseId = null 
            };
            vm.Questions = questions;

            ViewBag.ReturnQuizId = quiz.QuizId;

            return View(vm);
        }

        private async Task<bool> CanUserAccessQuizAsync(int quizId, User user)
        {
            if (User.IsInRole("Admin"))
                return true;

            if (await quizQueryService.CheckIfPublicAsync(quizId))
                return true;

            if (await accessValidationService.UserOwnsQuizAsync(quizId, user))
                return true;

            return await chapterRepository.UserHasEnrolledCourseWithQuizAsync(user.Id, quizId);
        }
    }
}