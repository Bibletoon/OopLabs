using System.Collections.Generic;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetNameJobBuilder
    {
        public ISetStorageAlgorithmJobBuilder SetName(string name);
    }
}