using System.Collections.Generic;
using System.IO;
using Backups.Entities;
using Backups.FileHandlers;
using NUnit.Framework;

namespace Backups.Tests.ArchiverTests
{
    public class ZipArchiverTests
    {
        private ZipFileArchiver _archiver;

        [SetUp]
        public void SetUp()
        {
            _archiver = new ZipFileArchiver(new TestLogger());
        }

        [Test]
        public void ArchiveAndDearchiveFiles_FilesShouldBeEqual()
        {
            using var fileAContent = new MemoryStream();
            var writerA = new StreamWriter(fileAContent, leaveOpen: true);
            writerA.Write("a.txt");
            writerA.Close();
            fileAContent.Seek(0, SeekOrigin.Begin);
            Package fileA = new Package("a.txt", fileAContent);

            using var fileBContent = new MemoryStream();
            var writerB = new StreamWriter(fileBContent, leaveOpen: true);
            writerB.Write("b.txt");
            writerB.Close();
            fileBContent.Seek(0, SeekOrigin.Begin);
            Package fileB = new Package("b.txt", fileBContent);

            using var resultStream = new MemoryStream();
            _archiver.ArchiveFiles(new List<Package>(){fileA, fileB}, resultStream);
            using var resultArchive = new Package("archive.zip", resultStream);
            var dearchivedFiles = _archiver.DearchiveFile(resultArchive);

            foreach (var file in dearchivedFiles)
            {
                var streamReader = new StreamReader(file.Content);
                var content = streamReader.ReadToEnd();
                streamReader.Close();
                Assert.AreEqual(file.Name, content);
            }

            dearchivedFiles.ForEach(f=>f.Dispose());
        }
    }
}