using System;
using System.Collections.Generic;
using System.IO;
using Backups.Entities;

namespace Backups.Storages
{
    public class LocalStorage : IStorage
    {
        public void WriteFiles(string folderPath, List<Package> files)
        {
            ArgumentNullException.ThrowIfNull(folderPath, nameof(folderPath));
            ArgumentNullException.ThrowIfNull(files, nameof(files));
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            foreach (var file in files)
            {
                using var fileStream = File.Open($"{folderPath}{Path.PathSeparator}{file.Name}", FileMode.Create);
                file.Content.CopyTo(fileStream);
            }
        }
    }
}