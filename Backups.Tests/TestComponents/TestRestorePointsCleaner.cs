using System.Collections.Generic;
using Backups.Entities;
using Backups.Models;
using Backups.RestorePointsCleaners;

namespace Backups.Tests.TestComponents
{
    public class TestRestorePointsCleaner : IRestorePointsCleaner
    {
        public List<JobObjectInfo> GetJobObjectsToKeep(List<RestorePointInfo> restorePoints, List<JobObject> jobObjects)
            => new List<JobObjectInfo>();
    }
}