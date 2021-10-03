using System;

namespace IsuExtra.Domain.Entities
{
    public class Faculty
    {
        public Faculty(string name, string groupPrefix)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(groupPrefix, nameof(groupPrefix));
            Name = name;
            GroupPrefix = groupPrefix;
        }

        public string Name { get; }
        public string GroupPrefix { get; }

        public static bool operator ==(Faculty left, Faculty right) => left?.Equals(right) ?? false;

        public static bool operator !=(Faculty left, Faculty right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Faculty faculty)
                return false;

            return Name.Equals(faculty.Name) && GroupPrefix.Equals(faculty.GroupPrefix);
        }

        public override int GetHashCode() => HashCode.Combine(Name, GroupPrefix);
    }
}