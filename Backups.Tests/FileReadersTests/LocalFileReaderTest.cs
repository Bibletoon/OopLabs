using System.IO;
using Backups.FileReaders;
using NUnit.Framework;

namespace Backups.Tests.FileReadersTests
{
    [TestFixture]
    public class LocalFileReaderTest
    {
        private IFileReader _reader;
        private const string _fileName = "a.txt";
        private const string _fileContent = "Some file content...";
        
        [OneTimeSetUp]
        public void SetUp()
        {
            _reader = new LocalFileReader();
            File.WriteAllText(_fileName, _fileContent);
        }

        [Test]
        public void ReadFile_ContentAndNameShouldBeProper()
        {
            using var file = _reader.ReadFile("a.txt");
            Assert.AreEqual(_fileName, file.Name);
            var contentReader = new StreamReader(file.Content);
            string content = contentReader.ReadToEnd();
            contentReader.Close();
            Assert.AreEqual(_fileContent, content);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            File.Delete("a.txt");
        }
    }
}