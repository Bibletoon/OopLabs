using System;
using Isu.Domain.Models;

namespace IsuExtra.Domain.Entities
{
    public class StudyCourse
    {
        public StudyCourse(CourseNumber courseNumber, Faculty faculty)
        {
            ArgumentNullException.ThrowIfNull(courseNumber, nameof(courseNumber));
            ArgumentNullException.ThrowIfNull(faculty, nameof(faculty));
            CourseNumber = courseNumber;
            Faculty = faculty;
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