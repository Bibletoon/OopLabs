using System;

namespace Isu.Domain.Models
{
    public class Student
    {
        internal Student(int id, string name, Group group)
        {
            Id = id;
            Name = name;
            Group = group;
        }

        public Group Group { get; private set; }
        public int Id { get; }
        public string Name { get; }

        public static bool operator ==(Student left, Student right) => left?.Equals(right) ?? false;

        public static bool operator !=(Student left, Student right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Student user)
                return false;

            return user.Id == Id && user.Name == Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);

        internal void ChangeGroup(Group newGroup)
        {
            Group = newGroup;
        }
    }
}