using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.Entities.Configuration;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;
using Backups.Storages;
using BackupsExtra.FileRestorers;

namespace BackupsExtra.Models
{
    public class RestoreJob
    {
        private readonly List<RestorePointInfo> _restorePoints;
        private readonly IStorage _storage;
        private readonly IFileArchiver _fileArchiver;
        private readonly IFileRestorer _fileRestorer;

        public RestoreJob(JobConfiguration jobConfiguration, IStorage storage, IFileArchiver fileArchiver, IFileRestorer fileRestorer)
        {
            _restorePoints = jobConfiguration.RestorePoints.ToList();
            _storage = storage;
            _fileArchiver = fileArchiver;
            _fileRestorer = fileRestorer;
        }

        public void Restore(string pointName)
        {
            var pointToRestore = _restorePoints.FirstOrDefault(w => w.Name == pointName);

            if (pointToRestore is null)
                throw new ArgumentException("Invalid point name");

            var archivedPackages = _storage.ReadFiles($"jobs/{pointToRestore.JobName}/{pointToRestore.Name}");
            var dearchivedPackages = archivedPackages.SelectMany(p => _fileArchiver.DearchiveFile(p)).ToList();
            var jobObjectInfos = pointToRestore.Objects
                                               .Select(o => new JobObjectInfo(
                                                           o,
                                                           dearchivedPackages.First(
                                                               p => p.Name == Path.GetFileName(o.Path)))).ToList();
            _fileRestorer.RestoreFiles(jobObjectInfos);
            archivedPackages.ForEach(p => p.Dispose());
            dearchivedPackages.ForEach(p => p.Dispose());
        }
    }
}