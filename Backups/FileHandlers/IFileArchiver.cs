using System.Collections.Generic;
using System.IO;
using Backups.Entities;

namespace Backups.FileHandlers
{
    public interface IFileArchiver
    {
        void ArchiveFiles(List<Package> files, Stream writeStream);

        List<Package> DearchiveFile(Package archiveContent);
        string GetArchiveExtension();
    }
}