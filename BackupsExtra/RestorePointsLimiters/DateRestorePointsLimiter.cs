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
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateRestorePointsLimiterConfig _config;

        public DateRestorePointsLimiter(IDateTimeProvider dateTimeProvider, DateRestorePointsLimiterConfig config)
        {
            _dateTimeProvider = dateTimeProvider;
            _config = config;
        }

        public List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects)
        {
            return createdPoints.Where(p => p.CreationDate <
                                            _dateTimeProvider.Now().Subtract(_config.RestorePointTimeLimit)).ToList();
        }
    }
}