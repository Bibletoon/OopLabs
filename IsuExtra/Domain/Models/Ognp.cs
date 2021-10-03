using System;
using System.Collections.Generic;
using IsuExtra.Domain.Entities;

namespace IsuExtra.Domain.Models
{
    public class Ognp
    {
        private readonly List<OgnpStream> _streams;

        internal Ognp(Faculty faculty, string name)
        {
            Faculty = faculty;
            Name = name;
            _streams = new List<OgnpStream>();
        }

        public Faculty Faculty { get; }
        public string Name { get; }
        public IReadOnlyList<OgnpStream> Streams => _streams.AsReadOnly();

        internal void AddStream(OgnpStream stream)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            _streams.Add(stream);
        }
    }
}