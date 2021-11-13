using Backups.Tools.Logger;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetLoggerJobBuilder
    {
        ISetDateTimeProviderJobBuilder SetLogger<T>()
            where T : class, ILogger;

        ISetDateTimeProviderJobBuilder SetLogger<T, TConfiguration>(TConfiguration configuration)
            where T : class, ILogger
            where TConfiguration : class;
    }
}