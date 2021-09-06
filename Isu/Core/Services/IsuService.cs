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
        private int _nextStudentId = 1;

        public IsuService(IsuServiceConfiguration configuration)
        {
            _configuration = configuration;
            _groups = new List<Group>();
            _students = new List<Student>();
        }

        public Group AddGroup(GroupName? name)
        {
            name = name.ThrowIfNull(new ArgumentNullException(nameof(name)));
            if (_groups.Any(g => g.CourseNumber == name.CourseNumber && g.GroupNumber == name.GroupNumber))
                throw new IsuException("Such group already exists");

            var group = new Group(name);
            _groups.Add(group);
            return group;
        }

        public Student AddStudent(Group? group, string name)
        {
            group = group.ThrowIfNull(new ArgumentNullException(nameof(group)));
            if (group.Students.Count >= _configuration.StudentsByGroupLimit)
                throw IsuException.GroupLimitReached();

            var student = new Student(_nextStudentId, name, group);
            _nextStudentId++;
            _students.Add(student);
            group.AddStudent(student);
            return student;
        }

        public Student GetStudent(int id) =>
            _students.Find(s => s.Id == id)
                           ?? throw new IsuException($"No student with such id - {id}");

        public Student? FindStudent(string name) =>
            _students.Find(s => s.Name == name);

        public List<Student> FindStudents(string groupName) =>
            _students.Where(s => s.Group.Name == groupName).ToList();

        public List<Student> FindStudents(CourseNumber courseNumber) =>
            _students.Where(s => s.Group.CourseNumber == courseNumber).ToList();

        public Group? FindGroup(string groupName) =>
            _groups.Find(g => g.Name == groupName);

        public List<Group> FindGroups(CourseNumber courseNumber) =>
            _groups.Where(g => g.CourseNumber == courseNumber).ToList();

        public void ChangeStudentGroup(Student? student, Group? newGroup)
        {
            student = student.ThrowIfNull(new ArgumentNullException(nameof(student)));
            newGroup = newGroup.ThrowIfNull(new ArgumentNullException(nameof(newGroup)));
            if (newGroup.Students.Count >= _configuration.StudentsByGroupLimit)
                throw IsuException.GroupLimitReached();

            Group? oldGroup = student.Group;
            oldGroup.RemoveStudent(student);
            newGroup.AddStudent(student);
            student.ChangeGroup(newGroup);
        }
    }
}