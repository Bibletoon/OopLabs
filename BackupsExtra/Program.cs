using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.Tools.BackupJobBuilder;
using BackupsExtra.DateTimeProviders;
using BackupsExtra.Loggers;
using BackupsExtra.Loggers.LoggerConfigurations;

var job = new BackupJobBuilder()
          .SetFileReader<LocalFileReader>()
          .SetName("job")
          .SetStorageAlgorithm<SingleStorageAlgorithm>()
          .SetStorage<LocalStorage>()
          .SetLogger<FileLogger, FileLoggerConfiguration>(new FileLoggerConfiguration("log.txt", true))
          .SetDateTimeProvider<RealDateTimeProvider>()
          .Build();

job.AddJobObject(new JobObject(@"C:\Users\alex8\Pictures\9pg5eq6w42d51.jpg"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Pictures\dolboeb.png"));
job.Run();