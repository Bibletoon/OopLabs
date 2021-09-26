namespace IsuExtra.Core.Configurations
{
    public class LessonsServiceConfiguration
    {
        public uint StudentsByOgnpStreamLimit { get; init; }
        public uint OgnpStreamsByStudentLimit { get; init; } = 2;
    }
}