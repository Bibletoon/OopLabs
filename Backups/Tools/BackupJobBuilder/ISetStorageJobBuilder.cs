using Backups.Storages;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetStorageJobBuilder
    {
        ISetLoggerJobBuilder SetStorage<T>()
            where T : class, IStorage;

        ISetLoggerJobBuilder SetStorage<T, TConfig>(TConfig config)
            where T : class, IStorage
            where TConfig : class;
    }
}