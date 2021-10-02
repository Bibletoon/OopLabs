using System;
using Isu.Domain.Models;
using IsuExtra.Domain.Services;

namespace IsuExtra.Core.Services
{
    public class MentorsService : IMentorsService
    {
        private int _nextMentorId;

        public MentorsService()
        {
            _nextMentorId = 1;
        }

        public Mentor AddMentor(string name)
        {
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            var mentor = new Mentor(_nextMentorId, name);
            _nextMentorId++;
            return mentor;
        }
    }
}