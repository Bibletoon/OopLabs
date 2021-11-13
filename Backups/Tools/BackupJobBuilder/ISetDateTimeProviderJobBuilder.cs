namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetDateTimeProviderJobBuilder
    {
        IFinalJobBuilder SetDateTimeProvider<T>()
            where T : class, IDateTimeProvider;
    }
}