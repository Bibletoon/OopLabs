using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Entities;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tools;
using Backups.Tools.Exceptions;
using Backups.Tools.Logger;
using Microsoft.Extensions.DependencyInjection;

namespace Backups.Models
{
    public class BackupJob
    {
        private readonly List<JobObject> _jobObjects;
        private readonly List<RestorePointInfo> _restorePoitnts;
        private readonly IFileReader _fileReader;
        private readonly IFileArchiver _fileArchiver;
        private readonly IStorageAlgorithm _storageAlgorithm;
        private readonly IStorage _storage;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILogger _logger;
        private string _name;

        public BackupJob(
            string name,
            IFileReader fileReader,
            IFileArchiver fileArchiver,
            IStorageAlgorithm storageAlgorithm,
            IStorage storage,
            IDateTimeProvider dateTimeProvider,
            ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(fileReader, nameof(fileReader));
            ArgumentNullException.ThrowIfNull(fileArchiver, nameof(fileArchiver));
            ArgumentNullException.ThrowIfNull(storageAlgorithm, nameof(storageAlgorithm));
            ArgumentNullException.ThrowIfNull(storage, nameof(storage));
            ArgumentNullException.ThrowIfNull(dateTimeProvider, nameof(dateTimeProvider));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));
            _jobObjects = new List<JobObject>();
            _restorePoitnts = new List<RestorePointInfo>();
            _name = name;
            _fileReader = fileReader;
            _fileArchiver = fileArchiver;
            _storageAlgorithm = storageAlgorithm;
            _storage = storage;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
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
            List<JobsGroup> fileGroups = _storageAlgorithm.ProceedFiles(_jobObjects);
            DateTime creationDateTime = _dateTimeProvider.Now();
            string backupName = creationDateTime.ToString("dd-MM-yyy-HH-mm-ss-ffff");

            string currentBackupPath = $"jobs/{_name}/{backupName}";

            var archives = new List<Package>();

            for (var index = 0; index < fileGroups.Count; index++)
            {
                JobsGroup filesToArchive = fileGroups[index];
                var filePaths = filesToArchive.Jobs.Select(s => s.Path).ToList();
                var archiveFileStream = new MemoryStream();

                var files = filePaths.Select(_fileReader.ReadFile).ToList();
                _fileArchiver.ArchiveFiles(files, archiveFileStream);
                archives.Add(new Package($"{index}.zip", archiveFileStream));
                files.ForEach(f => f.Dispose());
            }

            _storage.WriteFiles(currentBackupPath, archives);
            archives.ForEach(arch => arch.Dispose());

            _restorePoitnts.Add(new RestorePointInfo(creationDateTime, _jobObjects));
            _logger.Log($"Job {_name} finished");
        }

        internal BackupJobConfiguration GetConfiguration()
        {
            return new BackupJobConfiguration()
            {
                Name = _name,
                FileReader = _fileReader.GetType().FullName,
                StorageAlgorithm = _storageAlgorithm.GetType().FullName,
                Storage = _storageAlgorithm.GetType().FullName,
                JobObjects = _jobObjects.AsReadOnly(),
                RestorePoints = _restorePoitnts.AsReadOnly(),
            };
        }
    }
}