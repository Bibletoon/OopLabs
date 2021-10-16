using Backups.Domain.RestorePoitnts.RestorePointsCleaners;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetCleanerJobBuilder
    {
        IFinalJobBuilder SetRestorePointsCleaner<T>()
            where T : class, IRestorePointsCleaner;
    }
}