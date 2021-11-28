using System;

namespace BackupsExtra.RestorePointsLimiters.Configurations
{
    public class DateRestorePointsLimiterConfig
    {
        public DateRestorePointsLimiterConfig(TimeSpan restorePointTimeLimit)
        {
            RestorePointTimeLimit = restorePointTimeLimit;
        }

        public TimeSpan RestorePointTimeLimit { get; }
    }
}