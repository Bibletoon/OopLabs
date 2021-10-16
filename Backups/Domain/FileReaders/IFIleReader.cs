using Backups.Domain.Entities;

namespace Backups.Domain.FileReaders
{
    public interface IFileReader
    {
        ReadFileInfo ReadFile(string path);
    }
}