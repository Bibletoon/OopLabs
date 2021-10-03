using IsuExtra.Domain.Entities;

namespace IsuExtra.Domain.Models
{
    public class OgnpLesson : Lesson
    {
        internal OgnpLesson(LessonInfo lessonInfo, OgnpStream stream)
            : base(lessonInfo)
        {
            Stream = stream;
        }

        public OgnpStream Stream { get; }
    }
}