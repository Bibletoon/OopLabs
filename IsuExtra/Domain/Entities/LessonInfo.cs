using System;
using Isu.Domain.Models;
using IsuExtra.Tools;

namespace IsuExtra.Domain.Entities
{
    public class LessonInfo
    {
        public LessonInfo(LessonDateTimeSlot dateTimeSlot, int audienceNumber, Mentor mentor)
        {
            ArgumentNullException.ThrowIfNull(dateTimeSlot, nameof(dateTimeSlot));
            ArgumentNullException.ThrowIfNull(mentor, nameof(dateTimeSlot));

            if (audienceNumber <= 0)
                throw new LessonException("Incorrect audience number");

            DateTimeSlot = dateTimeSlot;
            AudienceNumber = audienceNumber;
            Mentor = mentor;
        }

        public LessonDateTimeSlot DateTimeSlot { get; }
        public int AudienceNumber { get; }
        public Mentor Mentor { get; }
    }
}