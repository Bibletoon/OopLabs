using System.Collections.Generic;
using System.Linq;
using Backups.Models;
using Backups.RestorePointsLimiters.Configurations;

namespace Backups.RestorePointsLimiters;

public class HybridRestorePointsLimiter : IRestorePointsLimiter
{
    private HybridRestorePointsLimiterConfig _config;

    public HybridRestorePointsLimiter(HybridRestorePointsLimiterConfig config)
    {
        _config = config;
    }

    public List<RestorePointInfo> GetPointsToClear(List<RestorePointInfo> createdPoints, List<JobObject> jobObjects)
    {
        var firstResult = _config.First.GetPointsToClear(createdPoints, jobObjects);
        var secondResult = _config.Second.GetPointsToClear(createdPoints, jobObjects);
        return _config.AcceptBoth ? firstResult.Union(secondResult).ToList() : firstResult.Intersect(secondResult).ToList();
    }
}