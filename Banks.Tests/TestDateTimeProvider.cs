using System;
using Banks.Tools;

namespace Banks.Tests
{
    public class TestDateTimeProvider : IDateTimeProvider
    {
        public DateTime CurrentDateTime { get; set; } = DateTime.Now;
        
        public DateTime Now() => CurrentDateTime;
    }
}