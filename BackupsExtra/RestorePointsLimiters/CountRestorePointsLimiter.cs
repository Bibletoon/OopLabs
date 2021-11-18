using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Models;
using Backups.RestorePointsLimiters;
using BackupsExtra.RestorePointsLimiters.Configurations;

namespace BackupsExtra.RestorePointsLimiters
{
    public class CountRestorePointsLimiter : IRestorePointsLimiter
    {
        public CountRestorePointsLimiter(CountRestorePointsLimiterConfiguration configuration)
        {
            Configuration = configuration;
        }

        public CountRestorePointsLimiterConfiguration Configuration { get; }

        public List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects)
        {
            return createdPoints.OrderByDescending(p => p.CreationDate)
                                .Skip(Configuration.MaximalRestorePointsCount - 1)
                                .ToList();
        }
    }
}