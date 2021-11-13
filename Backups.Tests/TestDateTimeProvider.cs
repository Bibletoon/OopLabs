using System;
using Backups.Tools;

namespace Backups.Tests
{
    public class TestDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now() => DateTime.Now;
    }
}