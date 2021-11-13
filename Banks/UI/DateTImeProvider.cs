using System;
using Banks.Tools;

namespace Banks.UI
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public TimeSpan Offset { get; set; } = TimeSpan.Zero;

        public DateTime Now() => DateTime.Now.Add(Offset);
    }
}