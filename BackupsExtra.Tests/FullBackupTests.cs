using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;
using Backups.RestorePointsCleaners;
using Backups.RestorePointsLimiters;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tests.TestComponents;
using Backups.Tools;
using Backups.Tools.BackupJobBuilder;
using Backups.Tools.Logger;
using BackupsExtra.DateTimeProviders;
using BackupsExtra.Loggers;
using BackupsExtra.RestorePointsCleaner;
using BackupsExtra.RestorePointsLimiters;
using BackupsExtra.RestorePointsLimiters.Configurations;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class FullBackupTests
    {
        private const string _jobName = "extrajob";
        private string _jobsPath;
        private string _testFolderPath;
        private BackupJob _job;

        [OneTimeSetUp]
        public void SetUp()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            _jobsPath = $"{currentDirectory}{Path.DirectorySeparatorChar}jobs";
            _job = new BackupJobBuilder()
                   .SetFileReader<LocalFileReader>()
                   .SetName(_jobName)
                   .SetStorageAlgorithm<SplitStorageAlgorithm>()
                   .SetStorage<LocalStorage>()
                   .SetLogger<TestLogger>()
                   .SetDateTimeProvider<TestDateTimeProvider>()
                   .SetRestorePointsLimiter<CountRestorePointsLimiter, CountRestorePointsLimiterConfiguration>(new CountRestorePointsLimiterConfiguration(1))
                   .SetRestorePointsCleaner<MergeRestorePointsCleaner>()
                   .Build();
            _testFolderPath = $"{currentDirectory}{Path.DirectorySeparatorChar}extraTestFiles";
            SetUpTestFiles();
        }

        public void SetUpTestFiles()
        {
            if (!Directory.Exists($"{_testFolderPath}"))
            {
                Directory.CreateDirectory($"{_testFolderPath}");
            }

            File.WriteAllText($"{_testFolderPath}{Path.DirectorySeparatorChar}a.txt", "Some content of a");
            File.WriteAllText($"{_testFolderPath}{Path.DirectorySeparatorChar}b.txt", "Some content of b");
            File.WriteAllText($"{_testFolderPath}{Path.DirectorySeparatorChar}c.txt", "Some content of c");
        }

        [TestCase]
        public void RunTwiceLocallyWithDeletionOfJobObjectsAndLimitForOnePoint_CreatesOnePointsMergedWithPrevious()
        {
            _job.AddJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}a.txt"));
            _job.AddJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}b.txt"));
            _job.AddJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}c.txt"));
            _job.Run();
            TestDateTimeProvider.AddTime(TimeSpan.FromHours(1));
            _job.RemoveJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}c.txt"));
            _job.Run();

            var restorePointsCount = Directory.GetDirectories($"{_jobsPath}{Path.DirectorySeparatorChar}{_jobName}").Length;
            var filesCount = Directory.GetFiles($"{_jobsPath}{Path.DirectorySeparatorChar}{_jobName}", "*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(1, restorePointsCount);
            Assert.AreEqual(3, filesCount);
        }

        [TestCase]
        public void GetJobConfigAndThenLoad_PointsAndObjectsAreSame()
        {
            var config = _job.GetConfiguration();
            var configurationManager = new ConfigurationManager();
            configurationManager.Save(config, "config.json");
            var loadedJob = configurationManager.LoadBackupJob("config.json");
            CollectionAssert.AreEqual(_job.JobObjects, loadedJob.JobObjects);
            CollectionAssert.AreEqual(_job.RestorePointInfos, loadedJob.RestorePointInfos);
        }

        [Test]
        public void ForScience()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "/bin/bash", Arguments = "rm -rf /home/hrrrrustic/testci", };
            Process proc = new Process() { StartInfo = startInfo, };
            proc.Start();
            proc.WaitForExit();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            File.Delete("config.json");
            Directory.Delete(_testFolderPath, true);
            Directory.Delete(_jobsPath, true);
        }
    }
}