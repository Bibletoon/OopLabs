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

        public static bool operator ==(FacultyName left, FacultyName right) => left?.Equals(right) ?? false;

        public static bool operator !=(FacultyName left, FacultyName right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not FacultyName facultyName)
                return false;

            return facultyName.Name.Equals(Name) && facultyName.GroupPrefix.Equals(GroupPrefix);
        }

        public override int GetHashCode() => HashCode.Combine(Name, GroupPrefix);
    }
}