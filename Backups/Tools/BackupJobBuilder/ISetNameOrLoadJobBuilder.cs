using System.Collections.Generic;
using Backups.Domain.Models;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetNameOrLoadJobBuilder
    {
        List<BackupJob> LoadJobs();

        public ISetStorageAlgorithmJobBuilder SetName(string name);
    }
}