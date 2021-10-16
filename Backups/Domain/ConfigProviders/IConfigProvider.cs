using System.Collections.Generic;
using Backups.Domain.Models;

namespace Backups.Domain.ConfigProviders
{
    public interface IConfigProvider
    {
        List<BackupJob> LoadJobs();

        void SaveJob(BackupJob job);
    }
}