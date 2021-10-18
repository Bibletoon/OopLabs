using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Backups.Domain.Entities;
using Backups.Domain.FileHandlers;
using Backups.Domain.FileReaders;
using Backups.Domain.Models;
using Backups.Domain.StorageAlgorithms;
using Backups.Domain.Storages;
using Backups.Tools.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Backups.Core.Storages
{
    public class LocalStorage : IStorage
    {
        private readonly IStorageAlgorithm _storageAlgorithm;
        private readonly IFileArchiver _fileArchiver;
        private readonly IFileReader _fileReader;
        private readonly IConfiguration _configuration;

        public LocalStorage(IStorageAlgorithm storageAlgorithm, IFileArchiver fileArchiver, IConfiguration configuration, IFileReader fileReader)
        {
            ArgumentNullException.ThrowIfNull(storageAlgorithm, nameof(storageAlgorithm));
            ArgumentNullException.ThrowIfNull(fileArchiver, nameof(fileArchiver));
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            _storageAlgorithm = storageAlgorithm;
            _fileArchiver = fileArchiver;
            _configuration = configuration;
            _fileReader = fileReader;
        }

        public RestorePointInfo CreateBackup(string jobName, List<JobObject> objects)
        {
            ArgumentNullException.ThrowIfNull(jobName);
            ArgumentNullException.ThrowIfNull(objects);

            List<JobsGroup> fileGroups = _storageAlgorithm.ProceedFiles(objects);
            DateTime creationDateTime = DateTime.Now;
            string backupName = creationDateTime.ToString("dd-MM-yyy-HH-mm-ss-ffff");

            if (string.IsNullOrEmpty(_configuration["jobsPath"]))
                throw BackupJobException.MissingConfigurationParameter("jobsPath");

            string currentBackupPath = $"{_configuration["jobsPath"]}/{jobName}/{backupName}";

            if (!Directory.Exists(currentBackupPath))
                Directory.CreateDirectory(currentBackupPath);

            for (var index = 0; index < fileGroups.Count; index++)
            {
                JobsGroup filesToArchive = fileGroups[index];
                var filePaths = filesToArchive.Jobs.Select(s => s.Path).ToList();
                using var archiveFileStream = new FileStream(
                    $"{currentBackupPath}/{index}.zip",
                    FileMode.Create);

                var files = filePaths.Select(_fileReader.ReadFile).ToList();
                _fileArchiver.ArchiveFiles(files, archiveFileStream);
                files.ForEach(f => f.Dispose());
            }

            return new RestorePointInfo(creationDateTime, objects);
        }
    }
}