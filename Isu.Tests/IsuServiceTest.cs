using System;
using System.Collections.Generic;
using Isu.Core.Configurations;
using Isu.Core.Services;
using Isu.Domain.Models;
using Isu.Domain.Services;
using Isu.Tools;
using NUnit.Framework;

namespace Isu.Tests
{
    public class Tests
    {
        private IIsuService _isuService;

        [SetUp]
        public void Setup()
        {
            var configuration = new IsuServiceConfiguration()
            {
                StudentsByGroupLimit = 3
            };
            _isuService = new IsuService(configuration);
        }

        [Test]
        public void ProvideNullArguments_TrowArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddGroup(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddStudent(null, "Name");
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.ChangeStudentGroup(null,null);
                                                });
        }

        [Test]
        public void CreateGroupTwice_ThrowException()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group groupOne = _isuService.AddGroup(groupName);

            Assert.Catch<IsuException>(() =>
                                       {
                                           _isuService.AddGroup(groupName);
                                       });
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Assert.AreEqual(group, student.Group);
            CollectionAssert.Contains(group.Students, student);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            _isuService.AddStudent(group, "Student A");
            _isuService.AddStudent(group, "Student B");
            _isuService.AddStudent(group, "Student C");

            Assert.Catch<IsuException>(() =>
                                       {
                                           _isuService.AddStudent(group, "Student D");
                                       });
        }

        [Test]
        public void CreateGroupInvalidGroupName_ThrowException()
        {
            Assert.Catch<IsuException>(() => GroupName.FromStringName("M3901"));
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            var newGroupName = GroupName.FromStringName("M3201");
            Group newGroup = _isuService.AddGroup(newGroupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");
            
            _isuService.ChangeStudentGroup(student, newGroup);
            
            Assert.AreEqual(newGroup, student.Group);
            CollectionAssert.Contains(newGroup.Students,student);
            CollectionAssert.DoesNotContain(group.Students, student);
        }

        [Test]
        public void TransferStudentToFullGroup_ThrowExceptionAndDontChangeGroup()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            _isuService.AddStudent(group, "Student A");
            _isuService.AddStudent(group, "Student B");
            _isuService.AddStudent(group, "Student C");
            var newGroupName = GroupName.FromStringName("M3201");
            Group newGroup = _isuService.AddGroup(newGroupName);
            Student student = _isuService.AddStudent(newGroup,"Student D");

            Assert.Throws<IsuException>(() =>
                                        {
                                            _isuService.ChangeStudentGroup(student, group);
                                        });
        }

        [Test]
        public void GetStudentById_ShouldReturnProperStudent()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Student gotStudent = _isuService.GetStudent(student.Id);
            
            Assert.AreEqual(student,gotStudent);
        }

        [Test]
        public void GetStudentByInvalidId_ThrowException()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Assert.Catch<IsuException>(() =>
                                       {
                                           _isuService.GetStudent(999);
                                       });
        }

        [Test]
        public void FindStudent_ShouldReturnProperStudent()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Student foundStudent = _isuService.FindStudent(student.Name);
            
            Assert.AreEqual(student.Id,foundStudent.Id);
        }
        
        [Test]
        public void FindStudentByInvalidName_ShouldReturnNull()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Assert.IsNull(_isuService.FindStudent("Ronimizy"));
        }

        [Test]
        public void FindStudentByGroupName_ShouldReturnProper()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            var newGroupName = GroupName.FromStringName("M3201");
            Group newGroup = _isuService.AddGroup(newGroupName);
            Student studentThree = _isuService.AddStudent(newGroup, "Valeruka");

            List<Student> students = _isuService.FindStudents(group.Name);
            
            CollectionAssert.Contains(students,studentOne);
            CollectionAssert.Contains(students,studentTwo);
            CollectionAssert.DoesNotContain(students,studentThree);
        }

        [Test]
        public void FindStudentsByInvalidGroupName_ShouldReturnEmptyList()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            List<Student> students = _isuService.FindStudents("M3201");
            
            CollectionAssert.IsEmpty(students);
        }

        [Test]
        public void FindStudentsByCourse_ShouldReturnProper()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            var newGroupName = GroupName.FromStringName("M3101");
            Group newGroup = _isuService.AddGroup(newGroupName);
            Student studentThree = _isuService.AddStudent(newGroup, "Valeruka");

            List<Student> students = _isuService.FindStudents(CourseNumber.Second);
            
            CollectionAssert.Contains(students,studentOne);
            CollectionAssert.Contains(students,studentTwo);
            CollectionAssert.DoesNotContain(students,studentThree);
        }
        
        [Test]
        public void FindStudentsByInvalidCourse_ShouldReturnEmptyList()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            List<Student> students = _isuService.FindStudents(CourseNumber.Fourth);
            
            CollectionAssert.IsEmpty(students);
        }

        [Test]
        public void FindGroupByName_ShouldReturnProper()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);

            Group newGroup = _isuService.FindGroup(group.Name);
            
            Assert.AreEqual(group,newGroup);
        }
        
        [Test]
        public void FindGroupByInvalidName_ShouldReturnNull()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);

            Assert.IsNull(_isuService.FindGroup("M3100"));
        }

        [Test]
        public void FindGroupsByCourseNumber_ShouldReturnProper()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            var anotherGroupName = GroupName.FromStringName("M3201");
            Group anotherGroup = _isuService.AddGroup(anotherGroupName);
            var thirdGroupName = GroupName.FromStringName("M3101");
            Group thirdGroup = _isuService.AddGroup(thirdGroupName);

            List<Group> groups = _isuService.FindGroups(CourseNumber.Second);
            
            CollectionAssert.Contains(groups,group);
            CollectionAssert.Contains(groups,anotherGroup);
            CollectionAssert.DoesNotContain(groups,thirdGroup);
        }

        [Test]
        public void FindGroupsByInvalidCourse_ShouldReturnEmptyList()
        {
            var groupName = GroupName.FromStringName("M3200");
            Group group = _isuService.AddGroup(groupName);
            
            List<Group> groups = _isuService.FindGroups(CourseNumber.Third);
            
            CollectionAssert.IsEmpty(groups);
        }
    }
}