using System;
using System.IO;
using System.Text.Json.Serialization;
using Backups.Tools;

namespace Backups.Entities
{
    [JsonConverter(typeof(PackageConverter))]
    public class Package : IDisposable
    {
        public Package(string name, Stream content)
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