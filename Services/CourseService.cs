using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class CourseService(ICourseRepository courseRepository, IModuleRepository moduleRepository,
        ICourseMapper courseMapper, IModuleMapper moduleMapper, IAccessValidationService accessValidationService,
        IFileStorageService fileStorageService, IQuizRepository quizRepository) : ICourseService
    {
        private async Task DeleteQuizImagesAsync(int? quizId)
        {
            if (!quizId.HasValue) return;
            var quiz = await quizRepository.GetQuizByIdAsync(quizId.Value);
            if (quiz is null) return;
            var tasks = quiz.Questions
                .Where(q => !string.IsNullOrEmpty(q.ImagePath))
                .Select(q => fileStorageService.DeleteAsync(q.ImagePath!));
            await Task.WhenAll(tasks);
        }
        public async Task<(bool success, int courseId, string error)> CreateAsync(CourseViewModel courseViewModel, int userId)
        {
            if (courseViewModel.IsPublic && await courseRepository.PublicTitleExistsAsync(courseViewModel.Title))
                return (false, 0, $"A public course named \"{courseViewModel.Title}\" already exists. Choose a different title.");

            var course = courseMapper.ToEntity(courseViewModel, userId);
            await courseRepository.CreateCourseAsync(course);
            return (true, course.CourseId, string.Empty);
        }

        public async Task<(bool success, string error)> DeleteAsync(int courseId)
        {
            // Load all modules (each includes Chapters) to collect R2 file paths before DB delete
            var modules = await moduleRepository.GetModulesByCourseIdAsync(courseId);
            foreach (var module in modules)
            {
                // Chapter file attachments + chapter quiz images
                foreach (var chapter in module.Chapters)
                {
                    if (!string.IsNullOrEmpty(chapter.FilePath))
                        await fileStorageService.DeleteAsync(chapter.FilePath);
                    await DeleteQuizImagesAsync(chapter.QuizId);
                }

                // Module roadmap file
                if (!string.IsNullOrEmpty(module.RoadmapFilePath))
                    await fileStorageService.DeleteAsync(module.RoadmapFilePath);

                // Module quiz images
                await DeleteQuizImagesAsync(module.QuizId);
            }

            var course = await courseRepository.GetCourseByIdAsync(courseId);
            await courseRepository.DeleteCourseAsync(course);
            return (true, string.Empty);
        }

        public async Task<CourseViewModel> GetEditAsync(int courseId)
        {
            var course = await courseRepository.GetCourseByIdAsync(courseId);
            var viewModel = courseMapper.ToCourseViewModel(course);

            var modules = await moduleRepository.GetModulesByCourseIdAsync(courseId);
            viewModel.Modules = modules.Select(m => moduleMapper.ToViewModel(m)).ToList();

            return viewModel;
        }

        public async Task<(bool success, IEnumerable<string> errors)> PostEditAsync(CourseViewModel courseViewModel, int userId)
        {
            if (courseViewModel.IsPublic && await courseRepository.PublicTitleExistsAsync(courseViewModel.Title, courseViewModel.CourseId))
                return (false, new[] { $"A public course named \"{courseViewModel.Title}\" already exists. Choose a different title." });

            var course = courseMapper.ToEntity(courseViewModel, userId);
            await courseRepository.UpdateCourseAsync(course);
            return (true, Enumerable.Empty<string>());
        }
    }
}
