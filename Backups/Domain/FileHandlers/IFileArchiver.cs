using System.Collections.Generic;
using System.IO;
using Backups.Domain.Entities;

namespace Backups.Domain.FileHandlers
{
    public interface IFileArchiver
    {
        void ArchiveFiles(List<ReadFileInfo> files, Stream writeStream);

        List<ReadFileInfo> DearchiveFile(ReadFileInfo archiveContent);
        string GetArchiveExtension();
    }
}