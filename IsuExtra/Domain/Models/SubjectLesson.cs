using Isu.Domain.Models;
using IsuExtra.Domain.Entities;

namespace IsuExtra.Domain.Models
{
    public class SubjectLesson : Lesson
    {
        internal SubjectLesson(LessonInfo lessonInfo, Subject subject, Group group)
            : base(lessonInfo)
        {
            Subject = subject;
            Group = group;
        }

        public Subject Subject { get; }
        public Group Group { get; }
    }
}