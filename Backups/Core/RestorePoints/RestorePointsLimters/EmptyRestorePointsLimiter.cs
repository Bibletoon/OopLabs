using System.Collections.Generic;
using Backups.Domain.Models;
using Backups.Domain.RestorePoitnts.RestorePointsLimters;

namespace Backups.Core.RestorePoints.RestorePointsLimters
{
    public class EmptyRestorePointsLimiter : IRestorePointsLimiter
    {
        public List<RestorePointInfo> GetPointsToClear(
            List<RestorePointInfo> existingPoints,
            List<JobObject> newObjects) => new List<RestorePointInfo>();
    }
}