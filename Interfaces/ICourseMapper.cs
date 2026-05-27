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
    }
}