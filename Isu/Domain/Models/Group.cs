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

        public CourseNumber CourseNumber => _name.CourseNumber;
        public int GroupNumber => _name.GroupNumber;
        public string Name => $"M3{CourseNumber}{GroupNumber:D2}";
        public IReadOnlyList<Student> Students => _students.AsReadOnly();

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