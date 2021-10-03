using System;
using System.Linq;
using Isu.Core.Configurations;
using Isu.Core.Services;
using Isu.Domain.Models;
using Isu.Domain.Services;
using IsuExtra.Core.Configurations;
using IsuExtra.Core.Services;
using IsuExtra.Domain.Entities;
using IsuExtra.Domain.Services;
using IsuExtra.Tools;
using NUnit.Framework;

namespace IsuExtra.Tests
{
    public class LessonsServiceTests
    {
        private ILessonsService _lessonsService;
        private IIsuService _isuService;
        private IMentorsService _mentorsService;
        
        [SetUp]
        public void Setup()
        {
            var configuration = new LessonsServiceConfiguration()
            {
                OgnpStreamsByStudentLimit = 2,
                StudentsByOgnpStreamLimit = 1,
            };
            _lessonsService = new LessonsService(configuration);
            _isuService = new IsuService(new IsuServiceConfiguration(){StudentsByGroupLimit = 20});
            _mentorsService = new MentorsService();
        }

        #region NullTests

        [Test]
        public void ProvideNullArgumentsToCtor_ThrowsException()
        {
            var slot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var mentor = _mentorsService.AddMentor("Makarevich");
            
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new LessonInfo(null, 100, mentor);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new LessonInfo(slot, 100, null);
                                                });

            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    new LessonsService(null);
                                                });
        }

        [Test]
        public void ProvideNullArgumentsToLessonServiceMethods_ThrowsException()
        {
            var faculty = new Faculty("TINT", "M");
            var course = new StudyCourse(CourseNumber.Second, faculty);
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "name");
            var mentor = _mentorsService.AddMentor("Makarevich");
            
            var anotherFaculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(anotherFaculty, "Name");
            var stream = _lessonsService.AddOgnpStream(ognp, "Name");
            var subject = _lessonsService.AddSubject(course, "name");
            var slot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var lessonInfo = new LessonInfo(slot, 100, mentor);
            
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddOgnp(null, "name");
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddOgnp(faculty, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddSubject(null, "name");
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddSubject(course, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddOgnpLesson(null, lessonInfo);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddOgnpLesson(stream, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddOgnpStream(null, "name");
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddOgnpStream(ognp, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddSubjectLesson(null, lessonInfo, group);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddSubjectLesson(subject, null, group);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.AddSubjectLesson(subject, lessonInfo, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.GetUnregisteredStudents(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.GetStudentOgnpStreams(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.GetSubjectsByCourse(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.GetSubjectLessonsByGroup(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.GetSubjectLessonsByMentor(null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.RegisterStudentToOgnpStream(null, stream);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.RegisterStudentToOgnpStream(student, null);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.UnregisterStudentFromOgnpStream(null, stream);
                                                });
            Assert.Catch<ArgumentNullException>(() =>
                                                {
                                                    _lessonsService.UnregisterStudentFromOgnpStream(student, null);
                                                });
        }

        #endregion

        #region SubjectsTests

        [Test]
        public void AddSubject_SubjectIsPresent()
        {
            var faculty = new Faculty("TINT", "M");
            var course = new StudyCourse(CourseNumber.Second, faculty);
            var subject = _lessonsService.AddSubject(course, "Subject");
            Assert.Contains(subject, _lessonsService.GetSubjectsByCourse(course).ToList());
        }

        [Test]
        public void AddSubjectLesson_LessonIsPresentInSubjectInGroupInMentor()
        {
            var faculty = new Faculty("TINT", "M");
            var course = new StudyCourse(CourseNumber.Second, faculty);
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var subject = _lessonsService.AddSubject(course, "Subject");
            var mentor = _mentorsService.AddMentor("Makarevich");
            var timeSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var lessonInfo = new LessonInfo(timeSlot, 100, mentor);
            var lesson = _lessonsService.AddSubjectLesson(subject, lessonInfo, group);
            
            Assert.Contains(lesson, _lessonsService.GetSubjectLessonsByGroup(group).ToList());
            Assert.Contains(lesson, _lessonsService.GetSubjectLessonsByMentor(mentor).ToList());
            Assert.Contains(lesson, subject.Lessons.ToList());
        }

        [Test]
        public void AddTwoSubjectLessonsWithoutIntersection_BothAdded()
        {
            var faculty = new Faculty("TINT", "M");
            var course = new StudyCourse(CourseNumber.Second, faculty);
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var subject = _lessonsService.AddSubject(course, "Subject");
            var mentor = _mentorsService.AddMentor("Makarevich");
            var timeSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var lessonInfo = new LessonInfo(timeSlot, 100, mentor);
            var timeSlotTwo = new LessonDateTimeSlot(new TimeOnly(10, 10), DayOfWeek.Friday);
            var lessonInfoTwo= new LessonInfo(timeSlotTwo, 100, mentor);
            var lesson = _lessonsService.AddSubjectLesson(subject, lessonInfo, group);
            var lessonTwo = _lessonsService.AddSubjectLesson(subject, lessonInfoTwo, group);
            
            Assert.Contains(lesson, _lessonsService.GetSubjectLessonsByGroup(group).ToList());
            Assert.Contains(lessonTwo, _lessonsService.GetSubjectLessonsByGroup(group).ToList());
        }

        [Test]
        public void AddTwoSubjectsWithAudienceAndDateTimeIntersection_ThrowsException()
        {
            var faculty = new Faculty("TINT", "M");
            var course = new StudyCourse(CourseNumber.Second, faculty);
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var subject = _lessonsService.AddSubject(course, "Subject");
            var mentor = _mentorsService.AddMentor("Makarevich");
            var timeSlot = new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday);
            var lessonInfo = new LessonInfo(timeSlot, 100, mentor);
            var timeSlotTwo = new LessonDateTimeSlot(new TimeOnly(10, 10), DayOfWeek.Monday);
            var lessonInfoTwo= new LessonInfo(timeSlotTwo, 100, mentor);
            var lesson = _lessonsService.AddSubjectLesson(subject, lessonInfo, group);

            Assert.Catch<LessonException>(() =>
                                    {
                                        _lessonsService.AddSubjectLesson(subject, lessonInfoTwo, group);
                                    });
        }

        #endregion

        #region OgnpTests

        [Test]
        public void AddNewOgnp_OgnpIsPresent()
        {
            var faculty = new Faculty("TINT", "M");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            Assert.Contains(ognp, _lessonsService.GetAllOgnps().ToList());
        }

        [Test]
        public void AddNewOgnpStream_OgnpStreamIsPresent()
        {
            var faculty = new Faculty("TINT", "M");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            
            Assert.Contains(stream, ognp.Streams.ToList());
        }

        [Test]
        public void RegisterStudentToOgnp_OgnpHasStudent()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");
            
            _lessonsService.RegisterStudentToOgnpStream(student, stream);
            
            Assert.Contains(student, stream.Students.ToList());
            Assert.Contains(stream, _lessonsService.GetStudentOgnpStreams(student).ToList());
        }

        [Test]
        public void RegisterStudentToOgnpOfHisFaculty_ThrowsException()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            var group = _isuService.AddGroup(GroupName.FromStringName("P3200"));
            var student = _isuService.AddStudent(group, "Student");

            Assert.Catch<OgnpException>(() =>
                                    {
                                        _lessonsService.RegisterStudentToOgnpStream(student, stream);
                                    });
        }

        [Test]
        public void RegisterStudentToAnotherStreamOfOgnpHeAlreadyRegistered_ThrowsException()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            var anotherStream = _lessonsService.AddOgnpStream(ognp, "Jopa");
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");
            _lessonsService.RegisterStudentToOgnpStream(student, stream);

            Assert.Catch<OgnpException>(() =>
                                    {
                                        _lessonsService.RegisterStudentToOgnpStream(student, anotherStream);
                                    });
        }

        [Test]
        public void RegisterStudentToThirdOgnp_ThrowsException()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            var anotherOngp = _lessonsService.AddOgnp(faculty, "MSPPO");
            var anotherStream = _lessonsService.AddOgnpStream(anotherOngp, "Name");
            var thirdOgnp = _lessonsService.AddOgnp(faculty, "Linuha");
            var thirdStream = _lessonsService.AddOgnpStream(thirdOgnp, "Name");
            
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");
            _lessonsService.RegisterStudentToOgnpStream(student, stream);
            _lessonsService.RegisterStudentToOgnpStream(student, anotherStream);

            Assert.Catch<OgnpException>(() =>
                                    {
                                        _lessonsService.RegisterStudentToOgnpStream(student, thirdStream);
                                    });
        }

        [Test]
        public void RegisterStudentToIntersectingOgnp_ThrowsException()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            var mentor = _mentorsService.AddMentor("Makarevich");
            
            var anotherFaculty = new Faculty("TINT", "M");
            var course = new StudyCourse(CourseNumber.Second, anotherFaculty);
            var subject = _lessonsService.AddSubject(course, "Subject");
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");

            var timeSlot = new LessonInfo(
                new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday), 
                100, mentor);

            var lessonTimeSlot = new LessonInfo(
                new LessonDateTimeSlot(new TimeOnly(10, 0), DayOfWeek.Monday),
                101, mentor);

            _lessonsService.AddOgnpLesson(stream, timeSlot);
            _lessonsService.AddSubjectLesson(subject, lessonTimeSlot, group);

            Assert.Catch<OgnpException>(() =>
                                    {
                                        _lessonsService.RegisterStudentToOgnpStream(student, stream);
                                    });
        }

        [Test]
        public void RegisterStudentToFullyPackedOgnp_ThrowsException()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");
            var anotherStudent = _isuService.AddStudent(group, "Student");
            _lessonsService.RegisterStudentToOgnpStream(student, stream);

            Assert.Catch<OgnpException>(() =>
                                    {
                                        _lessonsService.RegisterStudentToOgnpStream(anotherStudent, stream);
                                    });
        }

        [Test]
        public void UnregisterStudentFromHisOgnp_StudentNoMoreHasThisOgnp()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");
            
            _lessonsService.RegisterStudentToOgnpStream(student, stream);
            
            _lessonsService.UnregisterStudentFromOgnpStream(student, stream);
            
            Assert.IsEmpty(_lessonsService.GetStudentOgnpStreams(student));
            Assert.IsEmpty(stream.Students);
        }

        [Test]
        public void UnregisterStudentFromNotHisOgnp_ThrowsException()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");

            Assert.Catch<OgnpException>(() =>
                                    {
                                        _lessonsService.UnregisterStudentFromOgnpStream(student, stream);
                                    });
        }

        [Test]
        public void GetUnregesteredStudents_ShouldReturnProperResult()
        {
            var faculty = new Faculty("PIIKT", "P");
            var ognp = _lessonsService.AddOgnp(faculty, "Kiberbez");
            var stream = _lessonsService.AddOgnpStream(ognp, "Kekw");
            var anotherOngp = _lessonsService.AddOgnp(faculty, "MSPPO");
            var anotherStream = _lessonsService.AddOgnpStream(anotherOngp, "Name");
            
            var group = _isuService.AddGroup(GroupName.FromStringName("M3200"));
            var student = _isuService.AddStudent(group, "Student");
            var anotherStudent = _isuService.AddStudent(group, "Student");
            var thirdStudent = _isuService.AddStudent(group, "Student");
            _lessonsService.RegisterStudentToOgnpStream(student, stream);
            _lessonsService.RegisterStudentToOgnpStream(anotherStudent, anotherStream);
            var unregistered = _lessonsService.GetUnregisteredStudents(group);
            Assert.Contains(thirdStudent, unregistered.ToList());
            CollectionAssert.DoesNotContain(unregistered.ToList(), student);
            CollectionAssert.DoesNotContain(unregistered.ToList(), anotherStudent);
        }
        
        #endregion
    }
}