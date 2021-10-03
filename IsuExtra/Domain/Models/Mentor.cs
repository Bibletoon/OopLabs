using System;

namespace IsuExtra.Domain.Models
{
    public class Mentor
    {
        internal Mentor(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }

        public static bool operator ==(Mentor left, Mentor right) => left?.Equals(right) ?? false;

        public static bool operator !=(Mentor left, Mentor right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not Mentor user)
                return false;

            return user.Id == Id && user.Name == Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);
    }
}