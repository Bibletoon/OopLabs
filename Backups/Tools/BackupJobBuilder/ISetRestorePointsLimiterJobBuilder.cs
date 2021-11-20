using Backups.RestorePointsLimiters;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetRestorePointsLimiterJobBuilder
    {
        ISetRestorePointsCleanerJobBuilder SetRestorePointsLimiter<T>()
            where T : class, IRestorePointsLimiter;

        ISetRestorePointsCleanerJobBuilder SetRestorePointsLimiter<T, TConfig>(TConfig config)
            where T : class, IRestorePointsLimiter
            where TConfig : class;
    }
}