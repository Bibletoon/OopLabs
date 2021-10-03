using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Core.Configurations;
using Isu.Domain.Models;
using Isu.Domain.Services;
using Isu.Tools;

namespace Isu.Core.Services
{
    public class IsuService : IIsuService
    {
        private readonly List<Group> _groups;
        private readonly List<Student> _students;
        private readonly IsuServiceConfiguration _configuration;
        private int _nextUserId = 1;

        public IsuService(IsuServiceConfiguration configuration)
        {
            _configuration = configuration;
            _groups = new List<Group>();
            _groups = new List<Group>();
            _students = new List<Student>();
        }

        public Group AddGroup(GroupName name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            if (_groups.Any(g => g.FacultyPrefix == name.FacultyPrefix
                                 && g.CourseNumber == name.CourseNumber
                                 && g.GroupNumber == name.GroupNumber))
                throw new IsuException("Such group already exists");

            var group = new Group(name);
            _groups.Add(group);
            return group;
        }

        public Student AddStudent(Group group, string name)
        {
            ArgumentNullException.ThrowIfNull(group, nameof(group));
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            if (group.Students.Count >= _configuration.StudentsByGroupLimit)
                throw IsuException.GroupLimitReached();

            var student = new Student(_nextUserId, name, group);
            _nextUserId++;
            _students.Add(student);
            group.AddStudent(student);
            return student;
        }

        public Student GetStudent(int id) =>
            _students.Find(s => s.Id == id)
                           ?? throw new IsuException($"No student with such id - {id}");

        public Student FindStudent(string name) =>
            _students.Find(s => s.Name == name);

        public List<Student> FindStudents(string groupName) =>
            _students.Where(s => s.Group.Name == groupName).ToList();

        public List<Student> FindStudents(CourseNumber courseNumber) =>
            _students.Where(s => s.Group.CourseNumber == courseNumber).ToList();

        public Group FindGroup(string groupName) =>
            _groups.Find(g => g.Name == groupName);

        public List<Group> FindGroups(CourseNumber courseNumber) =>
            _groups.Where(g => g.CourseNumber == courseNumber).ToList();

        public void ChangeStudentGroup(Student student, Group newGroup)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            ArgumentNullException.ThrowIfNull(newGroup, nameof(newGroup));
            if (newGroup.Students.Count >= _configuration.StudentsByGroupLimit)
                throw IsuException.GroupLimitReached();

            if (student.Group is null)
                throw new IsuException("Student has no group");

            Group oldGroup = student.Group;
            oldGroup.RemoveStudent(student);
            newGroup.AddStudent(student);
            student.ChangeGroup(newGroup);
        }
    }
}