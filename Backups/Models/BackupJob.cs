using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.RestorePointsCleaners;
using Backups.RestorePointsLimiters;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tools;
using Backups.Tools.Exceptions;
using Backups.Tools.Logger;

namespace Backups.Models
{
    public class BackupJob
    {
        private readonly List<JobObject> _jobObjects;
        private readonly List<RestorePointInfo> _restorePoints;
        private readonly IRestorePointsLimiter _limiter;
        private readonly IRestorePointsCleaner _cleaner;
        private readonly IFileReader _fileReader;
        private readonly IFileArchiver _fileArchiver;
        private readonly IStorageAlgorithm _storageAlgorithm;
        private readonly IStorage _storage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private string _name;

        public BackupJob(
            string name,
            IRestorePointsLimiter limiter,
            IFileReader fileReader,
            IFileArchiver fileArchiver,
            IStorageAlgorithm storageAlgorithm,
            IStorage storage,
            IDateTimeProvider dateTimeProvider,
            ILogger logger,
            IRestorePointsCleaner cleaner)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(limiter, nameof(limiter));
            ArgumentNullException.ThrowIfNull(cleaner, nameof(cleaner));
            ArgumentNullException.ThrowIfNull(fileReader, nameof(fileReader));
            ArgumentNullException.ThrowIfNull(fileArchiver, nameof(fileArchiver));
            ArgumentNullException.ThrowIfNull(storageAlgorithm, nameof(storageAlgorithm));
            ArgumentNullException.ThrowIfNull(storage, nameof(storage));
            ArgumentNullException.ThrowIfNull(dateTimeProvider, nameof(dateTimeProvider));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            _jobObjects = new List<JobObject>();
            _restorePoints = new List<RestorePointInfo>();
            _name = name;
            _limiter = limiter;
            _fileReader = fileReader;
            _fileArchiver = fileArchiver;
            _storageAlgorithm = storageAlgorithm;
            _storage = storage;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _cleaner = cleaner;
        }

        public void AddJobObject(JobObject jobObject)
        {
            if (_jobObjects.Contains(jobObject))
                throw new BackupJobException("Such job object already added.");
            _jobObjects.Add(jobObject);
        }

        public void RemoveJobObject(JobObject jobObject)
        {
            _jobObjects.Remove(jobObject);
        }

        public void Run()
        {
            _logger.Log($"Starting job {_name}");

            var pointsToClear = _limiter.GetPointsToClear(_restorePoints, _jobObjects);

            var objectsToKeep = _cleaner.GetJobObjectsToKeep(pointsToClear, _jobObjects);

            var files = _jobObjects.Select(x => _fileReader.ReadFile(x.Path)).ToList();

            files.AddRange(objectsToKeep.Select(o => o.Package));

            pointsToClear.ForEach(p => _storage.RemoveFolder($"jobs/{p.JobName}/{p.Name}"));

            List<PackagesGroup> fileGroups = _storageAlgorithm.ProceedFiles(files);
            DateTime creationDateTime = _dateTimeProvider.Now();
            string backupName = creationDateTime.ToString("dd-MM-yyy-HH-mm-ss-ffff");

            string currentBackupPath = $"jobs/{_name}/{backupName}";
            var archives = new List<Package>();

            for (int index = 0; index < fileGroups.Count; index++)
            {
                PackagesGroup filesToArchive = fileGroups[index];
                var archiveFileStream = new MemoryStream();

                _fileArchiver.ArchiveFiles(filesToArchive.Packages, archiveFileStream);
                archives.Add(new Package($"{index}.zip", archiveFileStream));
                filesToArchive.Packages.ForEach(f => f.Dispose());
            }

            _storage.WriteFiles(currentBackupPath, archives);
            archives.ForEach(arch => arch.Dispose());

            _restorePoints.Add(new RestorePointInfo(creationDateTime, _name, backupName, _jobObjects.ToList()));
            _logger.Log($"Job {_name} finished");
        }
    }
}