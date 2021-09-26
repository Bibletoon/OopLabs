using System;
using IsuExtra.Domain.Entities;
using IsuExtra.Domain.Models;

namespace IsuExtra.Tools
{
    public class LessonException : Exception
    {
        public LessonException()
        {
        }

        public LessonException(string message)
            : base(message)
        {
        }

        public LessonException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static LessonException ScheduleIntersection()
            => new LessonException("Can't add lesson due to intersection");
    }
}