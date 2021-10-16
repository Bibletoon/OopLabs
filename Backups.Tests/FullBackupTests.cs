using System.Collections.Generic;
using System.IO;
using Backups.Core.ConfigProviders;
using Backups.Core.StorageAlgorithms;
using Backups.Core.Storages;
using Backups.Domain.Models;
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
            _jobsPath = $"{currentDirectory}\\jobs";
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "jobsPath", _jobsPath },
            }).Build();
            _job = new BackupJobBuilder(configuration)
                      .SetConfigProvider<InMemoryConfigProvider>()
                      .SetName(_jobName)
                      .SetStorageAlgorithm<SplitStorageAlgorithm>()
                      .SetStorage<LocalStorage>()
                      .Build();
            _testFolderPath = $"{currentDirectory}\\testFiles";
            SetUpTestFiles();
        }

        public void SetUpTestFiles()
        {
            if (!Directory.Exists($"{_testFolderPath}"))
            {
                Directory.CreateDirectory($"{_testFolderPath}");
            }

            File.WriteAllText($"{_testFolderPath}\\a.txt", "Some content of a");
            File.WriteAllText($"{_testFolderPath}\\b.txt", "Some content of b");
            File.WriteAllText($"{_testFolderPath}\\c.txt", "Some content of c");
        }

        [TestCase]
        public void RunTwiceLocallyWithDeletionOfJobObjects_CreatesTwoPointsAndProperAmountOfArchives()
        {
            _job.AddJobObject(new JobObject($"{_testFolderPath}\\a.txt"));
            _job.AddJobObject(new JobObject($"{_testFolderPath}\\b.txt"));
            _job.AddJobObject(new JobObject($"{_testFolderPath}\\c.txt"));
            _job.Run();
            _job.RemoveJobObject(new JobObject($"{_testFolderPath}\\c.txt"));
            _job.Run();

            var restorePointsCount = Directory.GetDirectories($"{_jobsPath}\\{_jobName}").Length;
            var filesCount = Directory.GetFiles($"{_jobsPath}\\{_jobName}", "*", SearchOption.AllDirectories).Length;
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