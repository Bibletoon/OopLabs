using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Backups.Entities;
using Backups.Tools.Logger;

namespace Backups.FileHandlers
{
    public class ZipFileArchiver : IFileArchiver
    {
        private readonly ILogger _logger;

        public ZipFileArchiver(ILogger logger)
        {
            _logger = logger;
        }

        public void ArchiveFiles(List<Package> files, Stream writeStream)
        {
            ArgumentNullException.ThrowIfNull(files, nameof(files));
            ArgumentNullException.ThrowIfNull(writeStream, nameof(writeStream));
            using var archive = new ZipArchive(writeStream, ZipArchiveMode.Create, leaveOpen: true);

            foreach (var file in files)
            {
                ZipArchiveEntry archiveEntry = archive.CreateEntry(file.Name);
                using Stream archiveEntryStream = archiveEntry.Open();
                file.Content.CopyTo(archiveEntryStream);
            }

            _logger.Log($"Files {string.Join(',', files.Select(f => f.Name))} archived");
        }

        public List<Package> DearchiveFile(Package archiveContent)
        {
            ArgumentNullException.ThrowIfNull(archiveContent, nameof(archiveContent));
            using var archive = new ZipArchive(archiveContent.Content, ZipArchiveMode.Read);

            var fileInfos = new List<Package>();

            foreach (var entry in archive.Entries)
            {
                var name = entry.FullName;
                var stream = new MemoryStream();
                using var entryStream = entry.Open();
                entryStream.CopyTo(stream);
                stream.Seek(0, SeekOrigin.Begin);
                fileInfos.Add(new Package(name, stream));
            }

            _logger.Log($"Files {string.Join(',', fileInfos.Select(f => f.Name))} archived");
            return fileInfos;
        }

        public string GetArchiveExtension() => "zip";
    }
}