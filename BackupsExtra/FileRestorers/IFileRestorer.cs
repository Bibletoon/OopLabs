using System.Collections.Generic;
using Backups.Entities;

namespace BackupsExtra.FileRestorers
{
    public interface IFileRestorer
    {
        void RestoreFiles(List<JobObjectInfo> objects);
    }
}