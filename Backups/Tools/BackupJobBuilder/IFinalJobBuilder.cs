using Backups.FileHandlers;
using Backups.FileReaders;
using Backups.Models;

namespace Backups.Tools.BackupJobBuilder
{
    public interface IFinalJobBuilder
    {
        IFinalJobBuilder SetFileArchiver<T>()
            where T : class, IFileArchiver;
        BackupJob Build();
    }
}