using System.Text.RegularExpressions;
using Isu.Tools;

namespace Isu.Domain.Models
{
    public class GroupName
    {
        private const string GroupNameRegex = "M3([1-4])([0-9]{2})$";

        public GroupName(CourseNumber courseNumber, int groupNumber)
        {
            if (groupNumber is <0 or >99)
            {
                throw new IsuException($"Invalid group number - {groupNumber}");
            }

            CourseNumber = courseNumber;
            GroupNumber = groupNumber;
        }

        public CourseNumber CourseNumber { get; init; }
        public int GroupNumber { get; init; }

        public static GroupName FromStringName(string name)
        {
            Match math = Regex.Match(name, GroupNameRegex);

            if (!math.Success)
            {
                throw new IsuException($"Invalid group name - {name}");
            }

            var courseNumber = (CourseNumber)int.Parse(math.Groups[1].Value);
            int groupNumber = int.Parse(math.Groups[2].Value);
            return new GroupName(courseNumber, groupNumber);
        }
    }
}