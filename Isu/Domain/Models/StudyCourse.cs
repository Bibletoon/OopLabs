using System;
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

        public static bool operator ==(StudyCourse left, StudyCourse right) => left?.Equals(right) ?? false;

        public static bool operator !=(StudyCourse left, StudyCourse right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not StudyCourse course)
                return false;

            return course.Faculty.Equals(Faculty) && course.CourseNumber.Equals(CourseNumber);
        }

        public override int GetHashCode() => HashCode.Combine(CourseNumber, Faculty);
    }
}