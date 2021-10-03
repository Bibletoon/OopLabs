using System;
using System.Collections.Generic;
using System.Linq;
using Isu.Domain.Models;
using IsuExtra.Core.Configurations;
using IsuExtra.Domain.Entities;
using IsuExtra.Domain.Models;
using IsuExtra.Domain.Services;
using IsuExtra.Tools;

namespace IsuExtra.Core.Services
{
    public class LessonsService : ILessonsService
    {
        private readonly LessonsServiceConfiguration _configuration;
        private readonly List<Subject> _subjects;
        private readonly List<SubjectLesson> _subjectLessons;
        private readonly List<Ognp> _ognps;
        private readonly List<OgnpStream> _ognpStreams;
        private readonly List<OgnpLesson> _ognpLessons;

        public LessonsService(LessonsServiceConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
            _configuration = configuration;
            _subjects = new List<Subject>();
            _subjectLessons = new List<SubjectLesson>();
            _ognps = new List<Ognp>();
            _ognpStreams = new List<OgnpStream>();
            _ognpLessons = new List<OgnpLesson>();
        }

        public Subject AddSubject(StudyCourse course, string name)
        {
            ArgumentNullException.ThrowIfNull(course, nameof(course));
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            var subject = new Subject(course, name);
            _subjects.Add(subject);
            return subject;
        }

        public SubjectLesson AddSubjectLesson(Subject subject, LessonInfo lessonInfo, Group group)
        {
            ArgumentNullException.ThrowIfNull(subject, nameof(subject));
            ArgumentNullException.ThrowIfNull(lessonInfo, nameof(lessonInfo));
            ArgumentNullException.ThrowIfNull(group, nameof(group));
            var subjectLesson = new SubjectLesson(lessonInfo, subject, group);

            if (GetAllLessons().Any(l => l.HasIntersection(subjectLesson)))
                throw LessonException.ScheduleIntersection();

            subject.AddLesson(subjectLesson);
            _subjectLessons.Add(subjectLesson);
            return subjectLesson;
        }

        public IReadOnlyList<Subject> GetSubjectsByCourse(StudyCourse course)
        {
            ArgumentNullException.ThrowIfNull(course, nameof(course));
            return _subjects.Where(s => s.StudyCourse == course).ToList();
        }

        public IReadOnlyList<SubjectLesson> GetSubjectLessonsByGroup(Group group)
        {
            ArgumentNullException.ThrowIfNull(group, nameof(group));
            return _subjectLessons.Where(s => s.Group == group).ToList();
        }

        public IReadOnlyList<SubjectLesson> GetSubjectLessonsByMentor(Mentor mentor)
        {
            ArgumentNullException.ThrowIfNull(mentor, nameof(mentor));
            return _subjectLessons.Where(s => s.Mentor == mentor).ToList();
        }

        public Ognp AddOgnp(Faculty faculty, string name)
        {
            ArgumentNullException.ThrowIfNull(faculty, nameof(faculty));
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            var ognp = new Ognp(faculty, name);
            _ognps.Add(ognp);
            return ognp;
        }

        public OgnpStream AddOgnpStream(Ognp ognp, string name)
        {
            ArgumentNullException.ThrowIfNull(ognp, nameof(ognp));
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            var ognpStream = new OgnpStream(ognp, name);

            ognp.AddStream(ognpStream);
            _ognpStreams.Add(ognpStream);
            return ognpStream;
        }

        public OgnpLesson AddOgnpLesson(OgnpStream stream, LessonInfo lessonInfo)
        {
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));
            ArgumentNullException.ThrowIfNull(lessonInfo, nameof(lessonInfo));

            var ognpLesson = new OgnpLesson(lessonInfo, stream);

            if (stream.Lessons.Any(l => l.HasIntersection(ognpLesson)))
                throw LessonException.ScheduleIntersection();

            stream.AddLesson(ognpLesson);
            _ognpLessons.Add(ognpLesson);
            return ognpLesson;
        }

        public IReadOnlyList<Ognp> GetAllOgnps() => _ognps.AsReadOnly();

        public void RegisterStudentToOgnpStream(Student student, OgnpStream stream)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            if (student.Group.FacultyPrefix.Equals(stream.Ognp.Faculty.GroupPrefix))
                throw new OgnpException("Can't register student to ognp of his faculty");

            if (GetStudentOgnpStreams(student).Any(s => s.Ognp == stream.Ognp))
                throw new OgnpException("Student is already registered to this ognp");

            if (GetStudentOgnpStreams(student).Count >= _configuration.OgnpStreamsByStudentLimit)
                throw OgnpException.StudentOgnpsLimitReached();

            if (stream.Students.Count >= _configuration.StudentsByOgnpStreamLimit)
                throw OgnpException.OgnpLimitReached();

            if (GetStudentLessons(student).Any(l => stream.Lessons.Any(sl => sl.HasTimeIntersection(l))))
                throw OgnpException.ScheduleIntersectionOnRegistration();

            stream.RegisterStudent(student);
        }

        public void UnregisterStudentFromOgnpStream(Student student, OgnpStream stream)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            ArgumentNullException.ThrowIfNull(stream, nameof(stream));

            stream.UnregisterStudent(student);
        }

        public IReadOnlyList<OgnpStream> GetStudentOgnpStreams(Student student)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            return _ognpStreams.Where(s => s.Students.Contains(student)).ToList();
        }

        public IReadOnlyList<Student> GetUnregisteredStudents(Group group)
        {
            ArgumentNullException.ThrowIfNull(group, nameof(group));
            return group.Students.Where(s => GetStudentOgnpStreams(s).Count == 0).ToList();
        }

        private IReadOnlyList<Lesson> GetAllLessons()
            => _subjectLessons.Cast<Lesson>().Concat(_ognpLessons).ToList();

        private IReadOnlyList<Lesson> GetStudentLessons(Student student)
        {
            ArgumentNullException.ThrowIfNull(student, nameof(student));
            var subjectLessons = GetSubjectLessonsByGroup(student.Group).Cast<Lesson>().ToList();
            GetStudentOgnpStreams(student).ToList().ForEach(l => subjectLessons.AddRange(l.Lessons));
            return subjectLessons;
        }
    }
}