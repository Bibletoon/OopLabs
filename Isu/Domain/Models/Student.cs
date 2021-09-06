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

        public int Id { get; init; }
        public string Name { get; init; }
        public Group Group { get; private set; }

        internal void ChangeGroup(Group newGroup)
        {
            Group = newGroup;
        }
    }
}