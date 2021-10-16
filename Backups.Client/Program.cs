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
job.AddJobObject(new JobObject(@"some_file"));

job.Run();