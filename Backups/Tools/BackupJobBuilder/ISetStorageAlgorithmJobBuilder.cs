using Backups.StorageAlgorithms;

namespace Backups.Tools.BackupJobBuilder
{
    public interface ISetStorageAlgorithmJobBuilder
    {
        ISetStorageJobBuilder SetStorageAlgorithm(IStorageAlgorithm algorithm);
    }
}