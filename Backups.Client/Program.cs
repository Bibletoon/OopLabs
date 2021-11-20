using System;
using System.Linq;
using Backups.Client;
using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;
using Backups.RestorePointsCleaners;
using Backups.RestorePointsLimiters;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.TcpServer.Common;
using Backups.Tools;
using Backups.Tools.BackupJobBuilder;
using Backups.Tools.Logger;
using BackupsExtra;
using BackupsExtra.DateTimeProviders;
using BackupsExtra.FileRestorers;
using BackupsExtra.FileRestorers.Configurations;
using BackupsExtra.Loggers;
using BackupsExtra.RestoreJobBuilder;
using BackupsExtra.RestorePointsCleaner;
using BackupsExtra.RestorePointsLimiters;
using BackupsExtra.RestorePointsLimiters.Configurations;

var config =  new ConnectionConfig("127.0.0.1", 8080);
var job = new BackupJobBuilder()
          .SetFileReader<LocalFileReader>()
          .SetName("job")
          .SetStorageAlgorithm<SingleStorageAlgorithm>()
          .SetStorage<TcpStorage, ConnectionConfig>(config)
          .SetLogger<EmptyLogger>()
          .SetDateTimeProvider<RealDateTimeProvider>()
          .SetRestorePointsLimiter<CountRestorePointsLimiter, CountRestorePointsLimiterConfiguration>(new CountRestorePointsLimiterConfiguration(1))
          .SetRestorePointsCleaner<MergeRestorePointsCleaner>()
          .Build();

var locator = new TypeLocator()
    .RegisterType<IFileArchiver>()
    .RegisterType<ZipFileArchiver>()
    .RegisterType<IFileReader>()
    .RegisterType<LocalFileReader>()
    .RegisterType<IStorageAlgorithm>()
    .RegisterType<SingleStorageAlgorithm>()
    .RegisterType<IStorage>()
    .RegisterType<LocalStorage>()
    .RegisterType<TcpStorage>()
    .RegisterType<ConnectionConfig>()
    .RegisterType<ILogger>()
    .RegisterType<EmptyLogger>()
    .RegisterType<IDateTimeProvider>()
    .RegisterType<RealDateTimeProvider>()
    .RegisterType<IRestorePointsLimiter>()
    .RegisterType<CountRestorePointsLimiter>()
    .RegisterType<CountRestorePointsLimiterConfiguration>()
    .RegisterType<IRestorePointsCleaner>()
    .RegisterType<MergeRestorePointsCleaner>();

job.AddJobObject(new JobObject(@"SomeFile"));
job.AddJobObject(new JobObject(@"AnotherFile"));
job.Run();
new ConfigurationManager().Save(job.GetConfiguration(), "config.json");
Console.WriteLine();
var restore = new RestoreJobBuilder()
              .LoadJobConfiguration(new ConfigurationManager(), locator, "config.json")
              .SetFileRestorer<OriginalPlaceFileRestorer>().Build();
restore.Restore(job.RestorePointInfos.First().Name);