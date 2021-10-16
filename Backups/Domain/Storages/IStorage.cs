using System.Collections.Generic;
using Backups.Domain.Models;
using Backups.Domain.RestorePoitnts.RestorePointsCleaners;

namespace Backups.Domain.Storages
{
    public interface IStorage
    {
        RestorePointInfo CreateBackup(string jobName, List<JobObject> objects);
        void CleanRestorePoints(List<RestorePointInfo> points, IRestorePointsCleaner cleaner);
    }
}