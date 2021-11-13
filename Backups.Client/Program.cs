using Backups.Client;
using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.TcpServer.Common;
using Backups.Tools.BackupJobBuilder;
using BackupsExtra.DateTimeProviders;
using BackupsExtra.Loggers;

var configuration = new ConnectionConfig("127.0.0.1", 8080);
var job = new BackupJobBuilder()
          .SetFileReader<LocalFileReader>()
          .SetName("job")
          .SetStorageAlgorithm<SingleStorageAlgorithm>()
          .SetStorage<TcpStorage, ConnectionConfig>(configuration)
          .SetLogger<EmptyLogger>()
          .SetDateTimeProvider<RealDateTimeProvider>()
          .Build();

job.AddJobObject(new JobObject(@"C:\Users\alex8\Pictures\9pg5eq6w42d51.jpg"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Pictures\dolboeb.png"));
job.Run();