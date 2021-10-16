using System.Collections.Generic;
using Backups.Domain.Models;

namespace Backups.Domain.RestorePoitnts.RestorePointsLimters
{
    public interface IRestorePointsLimiter
    {
        List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> existingPoints, List<JobObject> newObjects);
    }
}