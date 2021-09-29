using System;
using System.Collections.Generic;

namespace Isu.Domain.Models
{
    public class Group
    {
        private readonly GroupName _name;
        private readonly List<Student> _students;

        internal Group(GroupName name)
        {
            _name = name;
            _students = new List<Student>();
        }

        public Faculty Faculty => _name.StudyCourse.Faculty;
        public StudyCourse StudyCourse => _name.StudyCourse;
        public CourseNumber CourseNumber => _name.StudyCourse.CourseNumber;
        public int GroupNumber => _name.GroupNumber;
        public string Name => $"{Faculty.GroupPrefix}3{(int)CourseNumber}{GroupNumber:D2}";
        public IReadOnlyList<Student> Students => _students.AsReadOnly();

        public static bool operator ==(Group left, Group right) => left?.Equals(right) ?? false;

        public static bool operator !=(Group left, Group right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Group group)
                return false;

            return group.StudyCourse.Equals(StudyCourse) && group.Name.Equals(Name);
        }

        public override int GetHashCode() => HashCode.Combine(StudyCourse, Name);

        internal void AddStudent(Student student)
        {
            _students.Add(student);
        }

        internal void RemoveStudent(Student student)
        {
            _students.Remove(student);
        }
    }
}