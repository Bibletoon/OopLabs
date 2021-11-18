using System.Collections.Generic;
using System.Linq;
using Backups.Models;
using Backups.RestorePointsLimiters;
using Backups.Tools;
using BackupsExtra.RestorePointsLimiters.Configurations;

namespace BackupsExtra.RestorePointsLimiters
{
    public class DateRestorePointsLimiter : IRestorePointsLimiter
    {
        public DateRestorePointsLimiter(IDateTimeProvider dateTimeProvider, DateRestorePointsLimiterConfig config)
        {
            DateTimeProvider = dateTimeProvider;
            Config = config;
        }

        public IDateTimeProvider DateTimeProvider { get; set; }
        public DateRestorePointsLimiterConfig Config { get; set; }

        public List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects)
        {
            return createdPoints.Where(p => p.CreationDate <
                                            DateTimeProvider.Now().Subtract(Config.RestorePointTimeLimit)).ToList();
        }
    }
}