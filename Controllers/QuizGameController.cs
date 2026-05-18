using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Controllers
{
    public class QuizGameController(
        IAccessValidationService accessValidationService,
        UserManager<User> userManager,
        IQuizGameService quizGameService,
        IQuizQueryService quizQueryService,
        IChapterRepository chapterRepository) : Controller
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

            // Verify the user can access this quiz (same check as during play)
            if (!await CanUserAccessQuizAsync(quizId, user))
                return RedirectToAction("MyCourses", "Course");

            var (success, quizSummaryViewModel) = await quizGameService.AttemptSummary(quizId, user);

            if (!success)
                return RedirectToAction("MyCourses", "Course");

            return View(quizSummaryViewModel);
        }

        private async Task<bool> CanUserAccessQuizAsync(int quizId, User user)
        {
            if (User.IsInRole("Admin"))
                return true;

            if (await quizQueryService.CheckIfPublicAsync(quizId))
                return true;

            if (await accessValidationService.UserOwnsQuizAsync(quizId, user))
                return true;

            // Enrolled users can access quizzes attached to their courses
            return await chapterRepository.UserHasEnrolledCourseWithQuizAsync(user.Id, quizId);
        }
    }
}