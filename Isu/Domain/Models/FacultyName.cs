using System;

namespace Isu.Domain.Models
{
    public class FacultyName
    {
        public FacultyName(string name, string groupPrefix)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(groupPrefix, nameof(groupPrefix));
            Name = name;
            GroupPrefix = groupPrefix;
        }

        public string Name { get; }
        public string GroupPrefix { get; }
    }
}