using System.Collections.Generic;
using Backups.Tools.Logger;

namespace Backups.Tests
{
    public class TestLogger : ILogger
    {
        private readonly List<string> _messages = new List<string>();

        public IReadOnlyList<string> Messages => _messages.AsReadOnly();

        public void Log(string message)
        {
            _messages.Add(message);
        }
    }
}