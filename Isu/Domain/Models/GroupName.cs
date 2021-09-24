using System;
using System.Text.RegularExpressions;
using Isu.Tools;

namespace Isu.Domain.Models
{
    public class GroupName
    {
        public GroupName(StudyCourse studyCourse, int groupNumber)
        {
            ArgumentNullException.ThrowIfNull(studyCourse, nameof(studyCourse));
            if (groupNumber is <0 or >99)
            {
                throw new IsuException($"Invalid group number - {groupNumber}");
            }

            StudyCourse = studyCourse;
            GroupNumber = groupNumber;
        }

        public StudyCourse StudyCourse { get; }
        public int GroupNumber { get; }
    }
}