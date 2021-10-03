using System;
using IsuExtra.Domain.Entities;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    public class LessonDateTimeSlotTests
    {
        [Test]
        [TestCase(10, 0, 90)]
        public void CreateLessonDateTimeSlot_HasProperEndTime(int startHour,int startMinutes, int lessonDuration)
        {
            var startTime = new TimeOnly(startHour, startMinutes);
            var slot = new LessonDateTimeSlot(startTime, DayOfWeek.Monday);
            Assert.AreEqual(startTime.AddMinutes(lessonDuration), slot.EndTime);
        }
        
        #region IntersectionTests

        [Test]
        public void CheckDifferentDayDisjointTimeSlotsIntersection_ReturnFalse()
        {
            var firstSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var secondSlot = new LessonDateTimeSlot(new TimeOnly(15, 0), DayOfWeek.Wednesday);
            Assert.False(firstSlot.HasIntersection(secondSlot));
        }

        [Test]
        public void CheckDifferentDayIntersectingByTimeTimeSlotsIntersection_ReturnFalse()
        {
            var firstSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var secondSlot = new LessonDateTimeSlot(new TimeOnly(10, 30), DayOfWeek.Wednesday);
            Assert.False(firstSlot.HasIntersection(secondSlot));
        }
        
        [Test]
        public void CheckOneDayDisjointTimeSlotsIntersection_ReturnFalse()
        {
            var firstSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var secondSlot = new LessonDateTimeSlot(new TimeOnly(15, 0), DayOfWeek.Monday);
            Assert.False(firstSlot.HasIntersection(secondSlot));
        }

        [Test]
        public void CheckOneDayIntersectingTimeSlotsIntersection_ReturnTrue()
        {
            var firstSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var secondSlot = new LessonDateTimeSlot(new TimeOnly(10, 30), DayOfWeek.Monday);
            Assert.True(firstSlot.HasIntersection(secondSlot));
        }

        [Test]
        public void CheckOneDaySameTimeSlotsIntersection_ReturnTrue()
        {
            var firstSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var secondSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            Assert.True(firstSlot.HasIntersection(secondSlot));
        }

        #endregion
    }
}