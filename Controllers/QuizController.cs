using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quiz_project.Entities;
using quiz_project.Entities.Definition;
using quiz_project.Interfaces;
using quiz_project.ViewModels;


namespace quiz_project.Controllers
{

    public class QuizController(IQuizRepository quizRepository, IAccessValidationService accessValidationService,
                                 UserManager<User> userManager, IQuizService quizService, IQuizGameService quizGameService,
                                 IQuizQueryService quizQueryService, IFileStorageService fileStorageService,
                                 IAttemptRepository attemptRepository, IModuleRepository moduleRepository) : Controller
    {
        [HttpGet]
        [Authorize(Roles = "Creator,Admin")]
        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quizViewModels = User.IsInRole("Admin")
                ? await quizQueryService.GetAllQuizzesAsync()
                : await quizQueryService.GetUserQuizzesAsync(user.Id);

            return View(quizViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            var quizModel = new QuizViewModel
            {
                Title = quiz.Title,
                Description = quiz.Description
            };

            return View(quizModel);
        }

        [HttpGet, ActionName("Statistics")]
        public async Task<IActionResult> CheckQuizStats(int Id, string? returnUrl = null)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            if (!await quizQueryService.CheckIfPublicAsync(Id) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(Id, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var (success, quizStatisticsModel) = await quizQueryService.GetQuizStatisticsAsync(Id, user.Id);
            if (!success) return View("ZeroAttempts");

            ViewBag.ReturnUrl = returnUrl;
            return View(quizStatisticsModel);
        }

        [HttpGet, ActionName("Create")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> CreateNewQuizAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            return View();
        }

        [HttpPost, ActionName("Create")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> CreateNewQuizAsync(QuizViewModel quizViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user is null) return RedirectToAction("Register", "User")!;

                var (success, error) = await quizService.CreateAsync(quizViewModel, user.Id);
                if (!success)
                {
                    ModelState.AddModelError(String.Empty, error);
                    return View(quizViewModel);
                }

                return RedirectToAction("Index");
            }
            return View(quizViewModel);
        }

        [HttpGet, ActionName("Edit")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> EditQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var quizViewModel = await quizService.GetEditAsync(quizId);
            ViewBag.IsLocked = await quizService.HasAttemptsAsync(quizId);

            return View(quizViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> EditQuizAsync(QuizViewModel quizViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user is null) return RedirectToAction("Register", "User")!;
                if (!await quizQueryService.CheckIfPublicAsync(quizViewModel.QuizId) && !User.IsInRole("Admin"))
                {
                    var owns = await accessValidationService.UserOwnsQuizAsync(quizViewModel.QuizId, user);
                    if (!owns) return RedirectToAction("Index")!;
                }

                var (success, errors) = await quizService.PostEditAsync(quizViewModel, user.Id);

                if (!success)
                {
                    foreach (var er in errors)
                    {
                        ModelState.AddModelError(String.Empty, er);
                    }
                    return View(quizViewModel);
                }

                return RedirectToAction("Index");

            }
            return RedirectToAction("Edit");
        }

        [HttpPost, ActionName("Duplicate")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> DuplicateQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
            if (!owns && !User.IsInRole("Admin")) return RedirectToAction("Index")!;

            var (success, error) = await quizService.DuplicateAsync(quizId, user.Id);
            if (!success) TempData["Error"] = error;

            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> DeleteQuizAsync(int Id)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(Id) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(Id, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            try
            {
                await quizService.DeleteAsync(Id);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(String.Empty, $"Something went wrong: {e}");
            }

            return RedirectToAction("Index");
        }

        [HttpGet, ActionName("Game")]
        public async Task<IActionResult> LaunchQuizAsync(int quizId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;
            if (!await quizQueryService.CheckIfPublicAsync(quizId) && !User.IsInRole("Admin"))
            {
                var owns = await accessValidationService.UserOwnsQuizAsync(quizId, user);
                if (!owns) return RedirectToAction("Index")!;
            }

            var quizViewModel = await quizGameService.LaunchQuizAsync(quizId);

            return View(quizViewModel);
        }

        [HttpGet, ActionName("UserAttempts")]
        [Authorize(Roles = "Creator,Admin")]
        public async Task<IActionResult> UserAttemptsAsync(int userId, int courseId)
        {
            var creator = await userManager.GetUserAsync(User);
            if (creator is null) return RedirectToAction("Register", "User")!;

            var modules = await GetCourseModulesAsync(courseId);
            if (modules is null) return NotFound();

            var allQuizIds = modules
                .Select(m => m.QuizId).Where(id => id.HasValue).Select(id => id!.Value)
                .Concat(modules.SelectMany(m => m.Chapters.Select(c => c.QuizId).Where(id => id.HasValue).Select(id => id!.Value)))
                .Distinct().ToList();

            var attempts = await attemptRepository.GetUserAttemptsForQuizzesAsync(userId, allQuizIds);
            var quizDefs = await quizRepository.GetQuizzesByIdsAsync(allQuizIds);

            var contextMap = new Dictionary<int, string>();
            foreach (var m in modules)
            {
                if (m.QuizId.HasValue) contextMap[m.QuizId.Value] = $"Module: {m.Title}";
                foreach (var ch in m.Chapters)
                    if (ch.QuizId.HasValue) contextMap[ch.QuizId.Value] = $"Chapter: {ch.Title}";
            }

            var targetUser = await userManager.FindByIdAsync(userId.ToString());

            var vm = new UserAttemptsViewModel
            {
                UserId = userId,
                UserName = targetUser?.UserName ?? $"User #{userId}",
                CourseId = courseId,
                CourseTitle = $"Course #{courseId}",
                Attempts = attempts.Select(a =>
                {
                    quizDefs.TryGetValue(a.QuizId, out var qd);
                    return new UserAttemptItem
                    {
                        AttemptId = a.QuizAttemptId,
                        QuizTitle = qd?.Title ?? $"Quiz #{a.QuizId}",
                        Context = contextMap.GetValueOrDefault(a.QuizId, ""),
                        Score = a.Score,
                        TotalScore = qd?.TotalScore ?? 0,
                        HasPendingGrading = a.OpenAnswerRecords.Any(r => !r.IsGraded)
                    };
                }).ToList()
            };

            return View(vm);
        }

        [HttpGet, ActionName("AttemptDetail")]
        [Authorize(Roles = "Creator,Admin")]
        public async Task<IActionResult> AttemptDetailAsync(int attemptId, int? courseId)
        {
            var creator = await userManager.GetUserAsync(User);
            if (creator is null) return RedirectToAction("Register", "User")!;

            var attempt = await attemptRepository.GetAttemptFullDetailAsync(attemptId);
            if (attempt is null) return NotFound();

            if (!User.IsInRole("Admin") && !await accessValidationService.UserOwnsQuizAsync(attempt.QuizId, creator))
                return Forbid();

            var quiz = attempt.Quiz;
            var selectedAnswerIds = attempt.AnswerSelections
                .Select(s => s.AnswerId)
                .ToHashSet();

            var questions = quiz.Questions.Select(q =>
            {
                var openRecord = attempt.OpenAnswerRecords.FirstOrDefault(r => r.QuestionId == q.QuestionId);

                int earned = 0;
                bool correct = false;
                if (q.Type == Entities.Definition.QuestionType.MultipleChoice || q.Type == Entities.Definition.QuestionType.SingleChoice)
                {
                    var correctIds = q.Answers.Where(a => a.IsCorrect).Select(a => a.AnswerId).OrderBy(x => x).ToList();
                    var userIds = attempt.AnswerSelections.Where(s => s.QuestionId == q.QuestionId).Select(s => s.AnswerId).OrderBy(x => x).ToList();
                    correct = correctIds.SequenceEqual(userIds);
                    earned = correct ? q.QuestionScore : 0;
                }
                else if (openRecord != null)
                {
                    earned = openRecord.ManualScore ?? 0;
                    correct = earned > 0;
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
                CourseId = courseId,
                UserId = attempt.UserId,
                Questions = questions
            };

            return View(vm);
        }

        private async Task<List<Entities.Module>?> GetCourseModulesAsync(int courseId)
        {
            var modules = await moduleRepository.GetModulesByCourseIdAsync(courseId);
            return modules?.OrderBy(m => m.Order).ToList();
        }

        [HttpPost, ActionName("UploadQuestionImage")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> UploadQuestionImageAsync(IFormFile file)
        {
            if (file is null || file.Length == 0)
                return BadRequest(new { error = "No file." });

            var path = await fileStorageService.UploadAsync(file, 0, "quiz-images");
            return Ok(new { path });
        }

        [HttpGet, ActionName("QuestionImage")]
        public async Task<IActionResult> QuestionImageAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return NotFound();
            var (stream, contentType, _) = await fileStorageService.DownloadAsync(path);

            var ext = System.IO.Path.GetExtension(path).ToLowerInvariant();
            var mime = ext switch
            {
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
            return File(stream, mime);
        }

        [HttpGet, ActionName("PendingGrading")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> PendingGradingAsync()
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var quizzes = await quizQueryService.GetUserQuizzesAsync(user.Id);
            var quizIds = quizzes.Select(q => q.QuizId).ToList();

            var attempts = await attemptRepository.GetAttemptsWithUngradedAnswersAsync(quizIds);

            var vm = new PendingGradingViewModel
            {
                Attempts = attempts.Select(a => new PendingAttemptItem
                {
                    AttemptId = a.QuizAttemptId,
                    QuizTitle = a.Quiz?.Title ?? "-",
                    UserName = a.User?.UserName ?? "-",
                    UngradedCount = a.OpenAnswerRecords.Count(r => !r.IsGraded),
                    TotalOpenAnswers = a.OpenAnswerRecords.Count
                }).ToList()
            };

            return View(vm);
        }

        [HttpGet, ActionName("GradeAttempt")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> GradeAttemptAsync(int attemptId)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var attempt = await attemptRepository.GetAttemptWithOpenAnswersAsync(attemptId);
            if (attempt is null) return NotFound();

            if (!await accessValidationService.UserOwnsQuizAsync(attempt.QuizId, user))
                return Forbid();

            var quiz = await quizRepository.GetQuizByIdAsync(attempt.QuizId);

            var vm = new GradeAttemptViewModel
            {
                AttemptId = attempt.QuizAttemptId,
                QuizTitle = attempt.Quiz?.Title ?? "-",
                UserName = attempt.User?.UserName ?? "-",
                CurrentScore = attempt.Score,
                Answers = attempt.OpenAnswerRecords.Where(oa => !oa.IsGraded).Select(oa =>
                {
                    var q = quiz?.Questions.FirstOrDefault(x => x.QuestionId == oa.QuestionId);
                    return new OpenAnswerGradeItem
                    {
                        OpenAnswerRecordId = oa.Id,
                        QuestionDescription = q?.Description ?? "-",
                        ImagePath = q?.ImagePath,
                        MaxScore = q?.QuestionScore ?? 0,
                        OpenText = oa.OpenText,
                        ManualScore = oa.ManualScore,
                        IsGraded = oa.IsGraded
                    };
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost, ActionName("GradeAttempt")]
        [Authorize(Roles = "Creator")]
        public async Task<IActionResult> GradeAttemptPostAsync(int attemptId, List<int?> scores, List<int> recordIds)
        {
            var user = await userManager.GetUserAsync(User);
            if (user is null) return RedirectToAction("Register", "User")!;

            var attempt = await attemptRepository.GetAttemptWithOpenAnswersAsync(attemptId);
            if (attempt is null) return NotFound();
            if (!await accessValidationService.UserOwnsQuizAsync(attempt.QuizId, user))
                return Forbid();

            int oldSum = 0;
            var records = new List<OpenAnswerRecord>();
            for (int i = 0; i < recordIds.Count; i++)
            {
                var rec = attempt.OpenAnswerRecords.FirstOrDefault(r => r.Id == recordIds[i]);
                if (rec is null) continue;
                oldSum += rec.ManualScore ?? 0;
                rec.ManualScore = scores.ElementAtOrDefault(i) ?? 0;
                rec.IsGraded = true;
                records.Add(rec);
            }

            await attemptRepository.SaveOpenAnswerGradesAsync(records);

            int newSum = records.Sum(r => r.ManualScore ?? 0);
            int newScore = attempt.Score + (newSum - oldSum);
            await attemptRepository.UpdateScoreAsync(attemptId, newScore);

            return RedirectToAction("PendingGrading");
        }
    }
}