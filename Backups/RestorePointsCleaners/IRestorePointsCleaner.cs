using System.Collections.Generic;
using Backups.Entities;
using Backups.Models;

namespace Backups.RestorePointsCleaners
{
    public interface IRestorePointsCleaner
    {
        List<JobObjectInfo> GetJobObjectsToKeep(List<RestorePointInfo> restorePoints, List<JobObject> jobObjects);
    }
}