using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.FileHandlers;
using Backups.Models;
using Backups.RestorePointsCleaners;
using Backups.Storages;

namespace BackupsExtra.RestorePointsCleaner
{
    public class MergeRestorePointsCleaner : IRestorePointsCleaner
    {
        private readonly IStorage _storage;
        private readonly IFileArchiver _fileArchiver;

        public MergeRestorePointsCleaner(IStorage storage, IFileArchiver fileArchiver)
        {
            _storage = storage;
            _fileArchiver = fileArchiver;
        }

        public List<JobObjectInfo> GetJobObjectsToKeep(List<RestorePointInfo> restorePoints, List<JobObject> jobObjects)
        {
            if (restorePoints.Count == 0)
                return new List<JobObjectInfo>();

            var jobObjectsToKeep = new List<JobObjectInfo>();
            foreach (var restorePoint in restorePoints)
            {
                var archivedPackages = _storage.ReadFiles($"jobs/{restorePoint.JobName}/{restorePoint.Name}");
                var dearchivedPackages = archivedPackages.SelectMany(p => _fileArchiver.DearchiveFile(p)).ToList();
                var requiredFiles = restorePoint.Objects
                                                .Where(objectInRestorePoint =>
                                                           jobObjects.All(o => o.Path != objectInRestorePoint.Path) &&
                                                           jobObjectsToKeep.All(
                                                               j => j.JobObject.Path != objectInRestorePoint.Path))
                                                .ToList();

                var requiredJobObjectsInfo =
                    requiredFiles.Select(
                        f => new JobObjectInfo(f, dearchivedPackages.First(p => p.Name == Path.GetFileName(f.Path)))).ToList();
                jobObjectsToKeep.AddRange(requiredJobObjectsInfo);
                archivedPackages.ForEach(p => p.Dispose());
                dearchivedPackages.Where(p => requiredJobObjectsInfo.All(j => j.Package.Name != p.Name)).ToList().ForEach(p => p.Dispose());
            }

            return jobObjectsToKeep;
        }
    }
}