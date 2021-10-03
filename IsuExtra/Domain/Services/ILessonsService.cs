using System.Collections.Generic;
using Isu.Domain.Models;
using IsuExtra.Domain.Entities;
using IsuExtra.Domain.Models;

namespace IsuExtra.Domain.Services
{
    public interface ILessonsService
    {
        Subject AddSubject(StudyCourse course, string name);
        SubjectLesson AddSubjectLesson(Subject subject, LessonInfo lessonInfo, Group group);
        IReadOnlyList<Subject> GetSubjectsByCourse(StudyCourse course);

        IReadOnlyList<SubjectLesson> GetSubjectLessonsByGroup(Group group);

        IReadOnlyList<SubjectLesson> GetSubjectLessonsByMentor(Mentor mentor);

        Ognp AddOgnp(Faculty faculty, string name);
        OgnpStream AddOgnpStream(Ognp ognp, string name);
        OgnpLesson AddOgnpLesson(OgnpStream stream, LessonInfo lessonInfo);

        IReadOnlyList<Ognp> GetAllOgnps();

        void RegisterStudentToOgnpStream(Student student, OgnpStream stream);
        void UnregisterStudentFromOgnpStream(Student student, OgnpStream stream);
        IReadOnlyList<OgnpStream> GetStudentOgnpStreams(Student student);

        IReadOnlyList<Student> GetUnregisteredStudents(Group group);
    }
}