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

        public string FacultyPrefix => _name.FacultyPrefix;
        public CourseNumber CourseNumber => _name.CourseNumber;
        public int GroupNumber => _name.GroupNumber;
        public string Name => $"{_name.FacultyPrefix}3{(int)CourseNumber}{GroupNumber:D2}";
        public IReadOnlyList<Student> Students => _students.AsReadOnly();

        public static bool operator ==(Group left, Group right) => left?.Equals(right) ?? false;

        public static bool operator !=(Group left, Group right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Group group)
                return false;

            return group.Name.Equals(Name);
        }

        public override int GetHashCode() => Name.GetHashCode();

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