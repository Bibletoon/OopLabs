using Backups.Domain.FileReaders;
using Backups.Domain.Models;

namespace Backups.Tools.BackupJobBuilder
{
    public interface IFinalJobBuilder
    {
        IFinalJobBuilder SetFileReader<T>()
            where T : class, IFileReader;

        BackupJob Build();
    }
}