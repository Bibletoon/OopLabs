using Backups.RestorePointsCleaners;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetRestorePointsCleanerJobBuilder
    {
        IFinalJobBuilder SetRestorePointsCleaner<T>()
            where T : class, IRestorePointsCleaner;
    }
}