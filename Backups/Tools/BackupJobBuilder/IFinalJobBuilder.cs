using Backups.Domain.FileReaders;
using Backups.Domain.Models;
using Backups.Domain.RestorePoitnts.RestorePointsLimters;

namespace Backups.Tools.BackupJobBuilder
{
    public interface IFinalJobBuilder
    {
        ISetCleanerJobBuilder SetPointsLimiter<T>()
            where T : class, IRestorePointsLimiter;

        IFinalJobBuilder SetFileReader<T>()
            where T : class, IFileReader;

        BackupJob Build();
    }
}