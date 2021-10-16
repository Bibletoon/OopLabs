using System;
using System.IO;
using Backups.Domain.Entities;
using Backups.Domain.FileReaders;

namespace Backups.Core.FileReaders
{
    public class LocalFileReader : IFileReader
    {
        public ReadFileInfo ReadFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found at path {path}");

            return new ReadFileInfo(Path.GetFileName(path), File.OpenRead(path));
        }
    }
}