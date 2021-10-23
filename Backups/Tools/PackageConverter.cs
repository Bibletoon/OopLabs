using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Backups.Entities;

namespace Backups.Tools
{
    public class PackageConverter : JsonConverter<Package>
    {
        public override Package Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(typeToConvert, nameof(typeToConvert));
            var model = JsonSerializer.Deserialize<PackageModel>(ref reader, options);

            return new Package(model.Name, new MemoryStream(model.Data));
        }

        public override void Write(Utf8JsonWriter writer, Package value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(value, nameof(value));
            using var ms = new MemoryStream();
            value.Content.Position = 0;
            value.Content.CopyTo(ms);

            var model = new PackageModel()
            {
                Name = value.Name,
                Data = ms.ToArray(),
            };

            JsonSerializer.Serialize(writer, model, options);
        }

        private class PackageModel
        {
            public string Name { get; init; }
            public byte[] Data { get; init; }
        }
    }
}