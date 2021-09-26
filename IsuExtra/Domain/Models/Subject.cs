using System;
using System.Collections.Generic;
using Isu.Domain.Models;

namespace IsuExtra.Domain.Models
{
    public class Subject
    {
        private readonly List<SubjectLesson> _lessons;

        internal Subject(StudyCourse studyCourse, string name)
        {
            StudyCourse = studyCourse;
            Name = name;
            _lessons = new List<SubjectLesson>();
        }

        public StudyCourse StudyCourse { get; }
        public string Name { get; }
        public IReadOnlyList<SubjectLesson> Lessons => _lessons.AsReadOnly();

        internal void AddLesson(SubjectLesson lesson)
        {
            ArgumentNullException.ThrowIfNull(lesson, nameof(lesson));
            _lessons.Add(lesson);
        }
    }
}