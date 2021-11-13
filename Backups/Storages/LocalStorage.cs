using System;
using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.Tools.Logger;

namespace Backups.Storages
{
    public class LocalStorage : IStorage
    {
        private readonly ILogger _logger;

        public LocalStorage(ILogger logger)
        {
            _logger = logger;
        }

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
                _logger.Log($"File {file.Name} created");
            }
        }
    }
}