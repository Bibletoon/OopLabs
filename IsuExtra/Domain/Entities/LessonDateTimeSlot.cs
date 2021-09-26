using System;
using IsuExtra.Tools;

namespace IsuExtra.Domain.Entities
{
    public class LessonDateTimeSlot
    {
        public LessonDateTimeSlot(TimeOnly startTime, DayOfWeek dayOfWeek)
        {
            StartTime = startTime;
            EndTime = startTime.AddMinutes(90);

            // TODO: Change to concrete exception
            if (EndTime < StartTime)
                throw new LessonException("Unsupported lesson time");
            DayOfWeek = dayOfWeek;
        }

        public TimeOnly StartTime { get; }
        public TimeOnly EndTime { get; }
        public DayOfWeek DayOfWeek { get; }

        public bool HasIntersection(LessonDateTimeSlot slot)
        {
            ArgumentNullException.ThrowIfNull(slot, nameof(slot));
            return (slot.DayOfWeek == DayOfWeek)
                   && ((slot.StartTime >= StartTime && slot.StartTime <= EndTime)
                        || (slot.EndTime >= StartTime && slot.EndTime <= EndTime));
        }
    }
}