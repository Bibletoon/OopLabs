using System.Collections.Generic;
using Backups.Domain.Models;

namespace Backups.Domain.Storages
{
    public interface IStorage
    {
        RestorePointInfo CreateBackup(string jobName, List<JobObject> objects);
    }
}