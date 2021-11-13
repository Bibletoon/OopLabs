using System;

namespace Backups.Tools
{
    public interface IDateTimeProvider
    {
        DateTime Now();
    }
}