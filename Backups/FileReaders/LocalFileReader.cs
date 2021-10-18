using System;
using System.IO;
using Backups.Entities;

namespace Backups.FileReaders
{
    public class LocalFileReader : IFileReader
    {
        public Package ReadFile(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found at path {path}");

            return new Package(Path.GetFileName(path), File.OpenRead(path));
        }
    }
}