using Backups.Storages;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetStorageJobBuilder
    {
        IFinalJobBuilder SetStorage(IStorage storage);
    }
}