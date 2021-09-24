using System.Collections.Generic;

namespace Isu.Domain.Models
{
    public class Faculty
    {
        private readonly FacultyName _name;

        internal Faculty(FacultyName facultyName)
        {
            _name = facultyName;
        }

        public string Name => _name.Name;
        public string GroupPrefix => _name.GroupPrefix;
    }
}