using System.Collections.Generic;
using Backups.Client;
using Backups.FileReaders;
using Backups.Models;
using Backups.StorageAlgorithms;
using Backups.Storages;
using Backups.TcpServer.Common;
using Backups.Tools.BackupJobBuilder;
using Microsoft.Extensions.Configuration;

var configuration = new ConnectionConfig("127.0.0.1", 8080);
var job = new BackupJobBuilder()
          .SetFileReader(new LocalFileReader())
          .SetName("job")
          .SetStorageAlgorithm(new SingleStorageAlgorithm())
          .SetStorage(new TcpStorage(configuration))
          .Build();
job.AddJobObject(new JobObject(@"C:\Users\alex8\Pictures\9pg5eq6w42d51.jpg"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Pictures\dolboeb.png"));

job.Run();