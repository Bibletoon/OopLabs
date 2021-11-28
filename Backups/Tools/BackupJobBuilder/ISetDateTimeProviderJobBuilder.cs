namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetDateTimeProviderJobBuilder
    {
        ISetRestorePointsLimiterJobBuilder SetDateTimeProvider<T>()
            where T : class, IDateTimeProvider;
    }
}