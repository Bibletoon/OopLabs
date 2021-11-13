using Backups.StorageAlgorithms;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetStorageAlgorithmJobBuilder
    {
        ISetStorageJobBuilder SetStorageAlgorithm<T>()
            where T : class, IStorageAlgorithm;
    }
}