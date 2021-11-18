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
                using var fileStream = File.Open($"{folderPath}{Path.DirectorySeparatorChar}{file.Name}", FileMode.Create);
                file.Content.Seek(0, SeekOrigin.Begin);
                file.Content.CopyTo(fileStream);
                _logger.Log($"File {file.Name} created");
            }
        }

        public List<Package> ReadFiles(string folderPath)
        {
            ArgumentNullException.ThrowIfNull(folderPath, nameof(folderPath));

            if (!Directory.Exists(folderPath))
                return new List<Package>();

            var packages = new List<Package>();

            foreach (var filepath in Directory.EnumerateFiles(folderPath))
            {
                var ms = new MemoryStream();
                using var fs = File.OpenRead(filepath);
                fs.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);
                packages.Add(new Package(Path.GetFileName(filepath), ms));
            }

            return packages;
        }

        public void RemoveFolder(string folderPath)
        {
            ArgumentNullException.ThrowIfNull(folderPath, nameof(folderPath));
            if (!Directory.Exists(folderPath))
                return;

            Directory.Delete(folderPath, true);
        }
    }
}