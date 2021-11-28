using System;
using Backups.Tools;

namespace Backups.Tests.TestComponents
{
    public class TestDateTimeProvider : IDateTimeProvider
    {
        private static DateTime _dateTime = DateTime.Now;

        public DateTime Now() => _dateTime;

        public static void AddTime(TimeSpan timeSpan)
        {
            _dateTime = _dateTime.Add(timeSpan);
        }
    }
}