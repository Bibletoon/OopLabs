using System;

namespace Isu.Domain.Models
{
    public class IsuUser
    {
        protected IsuUser(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; }
        public string Name { get; }

        public static bool operator ==(IsuUser left, IsuUser right) => left?.Equals(right) ?? false;

        public static bool operator !=(IsuUser left, IsuUser right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (obj is not IsuUser user)
                return false;

            return user.Id == Id && user.Name == Name;
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name);
    }
}