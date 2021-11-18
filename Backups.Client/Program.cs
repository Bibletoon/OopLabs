using System.IO;
using System.Text.Json;
using Backups.Client;
using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.TcpServer.Common;
using Backups.Tools.BackupJobBuilder;
using BackupsExtra.DateTimeProviders;
using BackupsExtra.Loggers;
using BackupsExtra.RestorePointsCleaner;
using BackupsExtra.RestorePointsLimiters;
using BackupsExtra.RestorePointsLimiters.Configurations;

var configuration = new ConnectionConfig("127.0.0.1", 8080);
var job = new BackupJobBuilder()
          .SetFileReader<LocalFileReader>()
          .SetName("job")
          .SetStorageAlgorithm<SingleStorageAlgorithm>()
          .SetStorage<LocalStorage>()
          .SetLogger<EmptyLogger>()
          .SetDateTimeProvider<RealDateTimeProvider>()
          .SetRestorePointsCleaner<CountRestorePointsLimiter, CountRestorePointsLimiterConfiguration>(new CountRestorePointsLimiterConfiguration(1))
          .SetRestorePointsCleaner<MergeRestorePointsCleaner>()
          .Build();

job.AddJobObject(new JobObject(@"C:\Users\alex8\Downloads\test.txt"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Downloads\kekw.jpg"));
job.Run();
job.RemoveJobObject(new JobObject(@"C:\Users\alex8\Downloads\test.txt"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Downloads\ctffZtTWe46vubU_6o2WD.mp4"));
job.Run();