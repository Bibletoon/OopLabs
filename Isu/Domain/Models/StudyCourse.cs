using System.Collections.Generic;

namespace Isu.Domain.Models
{
    public class StudyCourse
    {
        private List<Group> _groups;

        internal StudyCourse(CourseNumber courseNumber, Faculty faculty)
        {
            CourseNumber = courseNumber;
            Faculty = faculty;
            _groups = new List<Group>();
        }

        public CourseNumber CourseNumber { get; }
        public Faculty Faculty { get; }
    }
}