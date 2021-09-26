namespace Isu.Domain.Models
{
    public class Student : IsuUser
    {
        internal Student(int id, string name, Group group)
            : base(id, name)
        {
            Group = group;
        }

        public Group Group { get; private set; }

        internal void ChangeGroup(Group newGroup)
        {
            Group = newGroup;
        }
    }
}