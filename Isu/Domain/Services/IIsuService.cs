using System.Collections.Generic;
using Isu.Domain.Models;

namespace Isu.Domain.Services
{
    public interface IIsuService
    {
        Group AddGroup(GroupName name);

        Student AddStudent(Group group, string name);

        Student GetStudent(int id);

        Student FindStudent(string name);

        List<Student> FindStudents(string groupName);

        List<Student> FindStudents(CourseNumber courseNumber);

        Group FindGroup(string groupName);

        List<Group> FindGroups(CourseNumber courseNumber);

        void ChangeStudentGroup(Student student, Group newGroup);
    }
}