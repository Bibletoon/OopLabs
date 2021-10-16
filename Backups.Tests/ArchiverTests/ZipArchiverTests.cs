using System.Collections.Generic;
using System.IO;
using Backups.Core.FileArchivers;
using Backups.Domain.Entities;
using NUnit.Framework;

namespace Backups.Tests.ArchiverTests
{
    public class ZipArchiverTests
    {
        private ZipFileArchiver _archiver;

        [SetUp]
        public void SetUp()
        {
            _archiver = new ZipFileArchiver();
        }
        
        [Test]
        public void ArchiveAndDearchiveFiles_FilesShouldBeEqual()
        {
            using var fileAContent = new MemoryStream();
            var writerA = new StreamWriter(fileAContent, leaveOpen: true);
            writerA.Write("a.txt");
            writerA.Close();
            fileAContent.Seek(0, SeekOrigin.Begin);
            ReadFileInfo fileA = new ReadFileInfo("a.txt", fileAContent);
            
            using var fileBContent = new MemoryStream();
            var writerB = new StreamWriter(fileBContent, leaveOpen: true);
            writerB.Write("b.txt");
            writerB.Close();
            fileBContent.Seek(0, SeekOrigin.Begin);
            ReadFileInfo fileB = new ReadFileInfo("b.txt", fileBContent);

            using var resultStream = new MemoryStream();
            _archiver.ArchiveFiles(new List<ReadFileInfo>(){fileA, fileB}, resultStream);
            using var resultArchive = new ReadFileInfo("archive.zip", resultStream);
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