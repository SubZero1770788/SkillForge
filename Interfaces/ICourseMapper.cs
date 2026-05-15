using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz_project.Entities;
using quiz_project.ViewModels;

namespace quiz_project.Interfaces
{
    public interface ICourseMapper
    {
        public Course ToEntity(CourseViewModel courseViewModel, int userId);
        public CourseViewModel ToCourseViewModel(Course course);
        // public CourseStatisticsModel ToCourseStatisticsModel(Course course, double averageScores, IEnumerable<CourseAttempt> courseAttempts,
        //                     CourseAttempt topUserAttempt, List<CourseAttempt> topScores,
        //                     Dictionary<int, string> users, Dictionary<(int QuestionId, int AnswerId), int>? answerCounts);
        // public CourseSummaryViewModel ToCourseSummaryViewModel(Course course, List<CourseAttempt> topScores,
        //                     Dictionary<int, string> users, CourseAttempt playerScore);
    }
}