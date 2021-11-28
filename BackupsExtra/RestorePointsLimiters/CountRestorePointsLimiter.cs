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
        private readonly CountRestorePointsLimiterConfiguration _configuration;

        public CountRestorePointsLimiter(CountRestorePointsLimiterConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects)
        {
            return createdPoints.OrderByDescending(p => p.CreationDate)
                                .Skip(_configuration.MaximalRestorePointsCount - 1)
                                .ToList();
        }
    }
}