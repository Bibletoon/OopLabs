using System.Collections.Generic;
using Backups.Client;
using Backups.Core.ConfigProviders;
using Backups.Core.StorageAlgorithms;
using Backups.Domain.Models;
using Backups.Tools.BackupJobBuilder;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>()
{
    { "jobsPath", "jobs" },
    { "serverIp", "127.0.0.1"},
    {"serverPort", "8080"}
}).Build();
var job = new BackupJobBuilder(configuration)
          .SetConfigProvider<InMemoryConfigProvider>()
          .SetName("job")
          .SetStorageAlgorithm<SingleStorageAlgorithm>()
          .SetStorage<TcpStorage>()
          .Build();
job.AddJobObject(new JobObject(@"C:\Users\alex8\Videos\videoplayback.mp4"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Downloads\про жопы.jpg"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Downloads\pudge.png"));
job.AddJobObject(new JobObject(@"C:\Users\alex8\Downloads\objection-1192312.mp4"));
job.Run();