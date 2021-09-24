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