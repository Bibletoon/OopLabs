using System;

namespace BackupsExtra.RestorePointsLimiters.Configurations
{
    public class CountRestorePointsLimiterConfiguration
    {
        public CountRestorePointsLimiterConfiguration(int maximalRestorePointsCount)
        {
            if (maximalRestorePointsCount < 1)
                throw new ArgumentException("You must keep at least one restore point");
            MaximalRestorePointsCount = maximalRestorePointsCount;
        }

        public int MaximalRestorePointsCount { get; }
    }
}