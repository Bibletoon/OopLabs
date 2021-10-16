using Backups.Domain.Storages;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetStorageJobBuilder
    {
        IFinalJobBuilder SetStorage<T>()
            where T : class, IStorage;
    }
}