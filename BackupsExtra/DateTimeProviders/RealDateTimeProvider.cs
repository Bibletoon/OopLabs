using System;
using Backups.Tools;

namespace BackupsExtra.DateTimeProviders
{
    public class RealDateTimeProvider : IDateTimeProvider
    {
        public DateTime Now() => DateTime.Now;
    }
}