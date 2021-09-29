using System;
using System.Collections.Generic;

namespace Isu.Domain.Models
{
    public class Faculty
    {
        private readonly FacultyName _name;

        internal Faculty(FacultyName facultyName)
        {
            _name = facultyName;
        }

        public string Name => _name.Name;
        public string GroupPrefix => _name.GroupPrefix;

        public static bool operator ==(Faculty left, Faculty right) => left?.Equals(right) ?? false;

        public static bool operator !=(Faculty left, Faculty right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Faculty faculty)
                return false;

            return faculty._name.Equals(_name);
        }

        public override int GetHashCode() => _name.GetHashCode();
    }
}