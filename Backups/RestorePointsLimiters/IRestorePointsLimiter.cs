using System.Collections.Generic;
using Backups.Models;

namespace Backups.RestorePointsLimiters
{
    public interface IRestorePointsLimiter
    {
        List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects);
    }
}