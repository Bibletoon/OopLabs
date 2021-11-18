using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.Models;

namespace Backups.Storages
{
    public interface IStorage
    {
        void WriteFiles(string folderPath, List<Package> files);

        void RemoveFolder(string folderPath);

        List<Package> ReadFiles(string folderPath);
    }
}