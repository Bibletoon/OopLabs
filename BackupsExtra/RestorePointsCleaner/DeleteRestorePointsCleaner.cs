using System.Collections.Generic;
using Backups.Entities;
using Backups.Models;
using Backups.RestorePointsCleaners;
using Backups.Storages;

namespace BackupsExtra.RestorePointsCleaner
{
    public class DeleteRestorePointsCleaner : IRestorePointsCleaner
    {
        private IStorage _storage;

        public DeleteRestorePointsCleaner(IStorage storage)
        {
            _storage = storage;
        }

        public List<JobObjectInfo> GetJobObjectsToKeep(List<RestorePointInfo> restorePoints, List<JobObject> jobObjects)
        {
            return new List<JobObjectInfo>();
        }
    }
}