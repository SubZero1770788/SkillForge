using quiz_project.Entities;
using quiz_project.Interfaces;
using quiz_project.ViewModels;

namespace quiz_project.Services
{
    public class CourseService(ICourseRepository courseRepository, IModuleRepository moduleRepository,
        ICourseMapper courseMapper, IModuleMapper moduleMapper, IAccessValidationService accessValidationService) : ICourseService
    {
        public async Task<(bool success, string error)> CreateAsync(CourseViewModel courseViewModel, int userId)
        {
            var course = courseMapper.ToEntity(courseViewModel, userId);
            await courseRepository.CreateCourseAsync(course);
            return (true, string.Empty);
        }

        public async Task<(bool success, string error)> DeleteAsync(int courseId)
        {
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
            var course = courseMapper.ToEntity(courseViewModel, userId);
            await courseRepository.UpdateCourseAsync(course);
            return (true, Enumerable.Empty<string>());
        }
    }
}
