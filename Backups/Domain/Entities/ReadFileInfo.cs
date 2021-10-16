using System;
using System.IO;

namespace Backups.Domain.Entities
{
    public class ReadFileInfo : IDisposable
    {
        public ReadFileInfo(string name, Stream content)
        {
            Name = name;
            Content = content;
        }

        public string Name { get; }
        public Stream Content { get; }

        public void Dispose()
        {
            Content?.Dispose();
        }
    }
}