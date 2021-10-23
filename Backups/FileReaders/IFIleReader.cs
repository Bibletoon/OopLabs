using Backups.Entities;

namespace Backups.FileReaders
{
    public interface IFileReader
    {
        Package ReadFile(string path);
    }
}