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
    }
}