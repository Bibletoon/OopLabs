using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Domain.Entities;
using Backups.Domain.FileHandlers;
using Backups.Domain.FileReaders;

namespace Backups.Core.FileArchivers
{
    public class ZipFileArchiver : IFileArchiver
    {
        public void ArchiveFiles(List<ReadFileInfo> files, Stream writeStream)
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
        }

        public List<ReadFileInfo> DearchiveFile(ReadFileInfo archiveContent)
        {
            ArgumentNullException.ThrowIfNull(archiveContent, nameof(archiveContent));
            using var archive = new ZipArchive(archiveContent.Content, ZipArchiveMode.Read);

            var fileInfos = new List<ReadFileInfo>();

            foreach (var entry in archive.Entries)
            {
                var name = entry.FullName;
                var stream = new MemoryStream();
                using var entryStream = entry.Open();
                entryStream.CopyTo(stream);
                stream.Seek(0, SeekOrigin.Begin);
                fileInfos.Add(new ReadFileInfo(name, stream));
            }

            return fileInfos;
        }

        public string GetArchiveExtension() => "zip";
    }
}