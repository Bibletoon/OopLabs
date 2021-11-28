using System.Collections.Generic;
using Backups.Models;
using Backups.RestorePointsLimiters;

namespace Backups.Tests.TestComponents
{
    public class TestRestorePointsLimiter : IRestorePointsLimiter
    {
        public List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects)
            => new List<RestorePointInfo>();
    }
}