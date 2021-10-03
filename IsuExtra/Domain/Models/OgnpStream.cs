using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Domain.Models;
using IsuExtra.Tools;

namespace IsuExtra.Domain.Models
{
    public class OgnpStream
    {
        private readonly List<OgnpLesson> _lessons;
        private readonly List<Student> _students;

        internal OgnpStream(Ognp ognp, string name)
        {
            Ognp = ognp;
            Name = name;
            _lessons = new List<OgnpLesson>();
            _students = new List<Student>();
        }

        public Ognp Ognp { get; }
        public string Name { get; }
        public IReadOnlyList<OgnpLesson> Lessons => _lessons.AsReadOnly();
        public IReadOnlyList<Student> Students => _students.AsReadOnly();

        internal void RegisterStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            _students.Add(student);
        }

        internal void AddLesson(OgnpLesson lesson)
        {
            ArgumentNullException.ThrowIfNull(lesson, nameof(lesson));
            _lessons.Add(lesson);
        }

        internal void UnregisterStudent(Student student)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            if (!Students.Contains(student))
                throw new OgnpException("Can't unregister student from ognp where he isn't registered");

            _students.Remove(student);
        }
    }
}