using System;
using IsuExtra.Domain.Entities;

namespace IsuExtra.Domain.Models
{
    public abstract class Lesson
    {
        private readonly LessonInfo _lessonInfo;

        protected Lesson(LessonInfo lessonInfo)
        {
            _lessonInfo = lessonInfo;
        }

        public LessonDateTimeSlot DateTimeSlot => _lessonInfo.DateTimeSlot;
        public int AudienceNumber => _lessonInfo.AudienceNumber;
        public Mentor Mentor => _lessonInfo.Mentor;

        public bool HasIntersection(Lesson lesson)
        {
            ArgumentNullException.ThrowIfNull(lesson, nameof(lesson));
            return AudienceNumber == lesson.AudienceNumber && DateTimeSlot.HasIntersection(lesson.DateTimeSlot);
        }

        public bool HasTimeIntersection(Lesson lesson)
        {
            ArgumentNullException.ThrowIfNull(lesson, nameof(lesson));
            return DateTimeSlot.HasIntersection(lesson.DateTimeSlot);
        }
    }
}