using System.Text.RegularExpressions;
using Isu.Tools;

namespace Isu.Domain.Models
{
    public class GroupName
    {
        private const string GroupNameRegex = "([A-Z])3([1-4])([0-9]{2})$";

        public GroupName(string facultyPrefix, CourseNumber courseNumber, int groupNumber)
        {
            if (!Regex.IsMatch(facultyPrefix, "[A-Z]"))
                throw new IsuException($"Invalid faculty prefix - {facultyPrefix}");

            if (groupNumber is <0 or >99)
                throw new IsuException($"Invalid group number - {groupNumber}");

            FacultyPrefix = facultyPrefix;
            CourseNumber = courseNumber;
            GroupNumber = groupNumber;
        }

        public string FacultyPrefix { get; init; }
        public CourseNumber CourseNumber { get; init; }
        public int GroupNumber { get; init; }

        public static GroupName FromStringName(string name)
        {
            Match match = Regex.Match(name, GroupNameRegex);

            if (!match.Success)
            {
                throw new IsuException($"Invalid group name - {name}");
            }

            string facultyPrefix = match.Groups[1].Value;
            var courseNumber = (CourseNumber)int.Parse(match.Groups[2].Value);
            int groupNumber = int.Parse(match.Groups[3].Value);
            return new GroupName(facultyPrefix, courseNumber, groupNumber);
        }
    }
}