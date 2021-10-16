using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Domain.ConfigProviders;
using Backups.Domain.Models;

namespace Backups.Core.ConfigProviders
{
    public class InMemoryConfigProvider : IConfigProvider
    {
        private readonly List<BackupJob> _jobs = new List<BackupJob>();

        public List<BackupJob> LoadJobs() => _jobs.ToList();

        public void SaveJob(BackupJob job)
        {
            ArgumentNullException.ThrowIfNull(job, nameof(job));
            _jobs.Add(job);
        }
    }
}