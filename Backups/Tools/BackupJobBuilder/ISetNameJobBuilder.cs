using System.Collections.Generic;
using Backups.Domain.Models;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetNameJobBuilder
    {
        public ISetStorageAlgorithmJobBuilder SetName(string name);
    }
}