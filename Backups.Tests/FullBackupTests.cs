using System.Collections.Generic;
using System.IO;
using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tools.BackupJobBuilder;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Backups.Tests
{
    [TestFixture]
    public class ModuleTests
    {
        private const string _jobName = "job";
        private string _jobsPath;
        private string _testFolderPath;
        private BackupJob _job;
        
        [OneTimeSetUp]
        public void SetUp()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            _jobsPath = $"{currentDirectory}{Path.DirectorySeparatorChar}jobs";
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "jobsPath", _jobsPath },
            }).Build();
            _job = new BackupJobBuilder()
                      .SetFileReader(new LocalFileReader())
                      .SetName(_jobName)
                      .SetStorageAlgorithm(new SplitStorageAlgorithm())
                      .SetStorage(new LocalStorage())
                      .Build();
            _testFolderPath = $"{currentDirectory}{Path.DirectorySeparatorChar}testFiles";
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
        public void RunTwiceLocallyWithDeletionOfJobObjects_CreatesTwoPointsAndProperAmountOfArchives()
        {
            _job.AddJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}a.txt"));
            _job.AddJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}b.txt"));
            _job.AddJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}c.txt"));
            _job.Run();
            _job.RemoveJobObject(new JobObject($"{_testFolderPath}{Path.DirectorySeparatorChar}c.txt"));
            _job.Run();

            var restorePointsCount = Directory.GetDirectories($"{_jobsPath}{Path.DirectorySeparatorChar}{_jobName}").Length;
            var filesCount = Directory.GetFiles($"{_jobsPath}{Path.DirectorySeparatorChar}{_jobName}", "*", SearchOption.AllDirectories).Length;
            Assert.AreEqual(2, restorePointsCount);
            Assert.AreEqual(5, filesCount);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Directory.Delete(_testFolderPath, true);
            Directory.Delete(_jobsPath, true);
        }
    }
}