using Backups.RestorePointsLimiters;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetRestorePointsLimiterJobBuilder
    {
        ISetRestorePointsCleanerJobBuilder SetRestorePointsCleaner<T>()
            where T : class, IRestorePointsLimiter;

        ISetRestorePointsCleanerJobBuilder SetRestorePointsCleaner<T, TConfig>(TConfig config)
            where T : class, IRestorePointsLimiter
            where TConfig : class;
    }
}