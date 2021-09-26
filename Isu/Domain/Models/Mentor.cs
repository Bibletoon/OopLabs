namespace Isu.Domain.Models
{
    public class Mentor : IsuUser
    {
        internal Mentor(int id, string name)
            : base(id, name)
        { }
    }
}