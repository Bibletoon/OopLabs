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
        public void ProvideNullArgumentsToIsuProvider_TrowArgumentNullException()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            var student = _isuService.AddStudent(group, "Name");
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddFaculty(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddStudyCourse(CourseNumber.Second,null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddGroup(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddStudent(group, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.AddStudent(null, "Name");
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.ChangeStudentGroup(null,group);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _isuService.ChangeStudentGroup(student, null);
                                                });
        }

        [Test]
        public void ProvideNullArgumentsToCtors_ThorwArgumentNullException()
        {
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new GroupName(null, 10);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new FacultyName(null, "M");
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new FacultyName("TINT", null);
                                                });
        }

        [Test]
        public void AddFacultyTwice_ThrowException()
        {
            var name = new FacultyName("TINT", "M");
            _isuService.AddFaculty(name);

            Assert.Catch<IsuException>(() =>
                                       {
                                           _isuService.AddFaculty(name);
                                       });
        }

        [Test]
        public void AddStudyCourseTwice_ThrowException()
        {
            var name = new FacultyName("TINT", "M");
            var faculty = _isuService.AddFaculty(name);
            _isuService.AddStudyCourse(CourseNumber.Second, faculty);

            Assert.Catch<IsuException>(() =>
                                       {
                                           _isuService.AddStudyCourse(CourseNumber.Second, faculty);
                                       });
        }

        [Test]
        public void CreateGroupTwice_ThrowException()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);

            Assert.Catch<IsuException>(() =>
                                       {
                                           _isuService.AddGroup(groupName);
                                       });
        }

        [Test]
        public void AddStudentToGroup_StudentHasGroupAndGroupContainsStudent()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Assert.AreEqual(group, student.Group);
            CollectionAssert.Contains(group.Students, student);
        }

        [Test]
        public void ReachMaxStudentPerGroup_ThrowException()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
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
        public void CreateInvalidGroupName_ThrowException()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            Assert.Catch<IsuException>(() => new GroupName(course, 111));
        }

        [Test]
        public void CreateGroup_GroupNameGeneratedProperly()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            var group = _isuService.AddGroup(groupName);
            Assert.AreEqual("M3200", group.Name);
        }

        [Test]
        public void TransferStudentToAnotherGroup_GroupChanged()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            var newGroupName = new GroupName(course, 1);
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
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            _isuService.AddStudent(group, "Student A");
            _isuService.AddStudent(group, "Student B");
            _isuService.AddStudent(group, "Student C");
            var newGroupName = new GroupName(course, 1);
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
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Student gotStudent = _isuService.GetStudent(student.Id);
            
            Assert.AreEqual(student,gotStudent);
        }

        [Test]
        public void GetStudentByInvalidId_ThrowException()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
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
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Student foundStudent = _isuService.FindStudent(student.Name);
            
            Assert.AreEqual(student.Id,foundStudent.Id);
        }
        
        [Test]
        public void FindStudentByInvalidName_ShouldReturnNull()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student student = _isuService.AddStudent(group, "Bibletoon");

            Assert.IsNull(_isuService.FindStudent("Ronimizy"));
        }

        [Test]
        public void FindStudentByGroupName_ShouldReturnProper()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");

            var newGroupName = new GroupName(course, 1);
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
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            List<Student> students = _isuService.FindStudents("M3201");
            
            CollectionAssert.IsEmpty(students);
        }

        [Test]
        public void FindStudentsByCourse_ShouldReturnProper()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            var newCourse = _isuService.AddStudyCourse(CourseNumber.First, faculty);
            var newGroupName = new GroupName(newCourse, 1);
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
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            Student studentOne = _isuService.AddStudent(group, "Bibletoon");
            Student studentTwo = _isuService.AddStudent(group, "Ronimizy");
            
            List<Student> students = _isuService.FindStudents(CourseNumber.Fourth);
            
            CollectionAssert.IsEmpty(students);
        }

        [Test]
        public void FindGroupByName_ShouldReturnProper()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);

            Group newGroup = _isuService.FindGroup(group.Name);
            
            Assert.AreEqual(group,newGroup);
        }
        
        [Test]
        public void FindGroupByInvalidName_ShouldReturnNull()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);

            Assert.IsNull(_isuService.FindGroup("M3100"));
        }

        [Test]
        public void FindGroupsByCourseNumber_ShouldReturnProper()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            var anotherGroupName = new GroupName(course, 1);
            Group anotherGroup = _isuService.AddGroup(anotherGroupName);
            var newCourse = _isuService.AddStudyCourse(CourseNumber.First, faculty);
            var thirdGroupName = new GroupName(newCourse, 2);
            Group thirdGroup = _isuService.AddGroup(thirdGroupName);

            List<Group> groups = _isuService.FindGroups(CourseNumber.Second);
            
            CollectionAssert.Contains(groups,group);
            CollectionAssert.Contains(groups,anotherGroup);
            CollectionAssert.DoesNotContain(groups,thirdGroup);
        }

        [Test]
        public void FindGroupsByInvalidCourse_ShouldReturnEmptyList()
        {
            var faculty = _isuService.AddFaculty(new FacultyName("TINT", "M"));
            var course = _isuService.AddStudyCourse(CourseNumber.Second, faculty);
            var groupName = new GroupName(course, 0);
            Group group = _isuService.AddGroup(groupName);
            
            List<Group> groups = _isuService.FindGroups(CourseNumber.Third);
            
            CollectionAssert.IsEmpty(groups);
        }
    }
}