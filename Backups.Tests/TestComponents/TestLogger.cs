using System.Collections.Generic;
using Backups.Tools.Logger;

namespace Backups.Tests.TestComponents
{
    public class TestLogger : ILogger
    {
        private List<string> _messages = new List<string>();

        public void Log(string message)
        {
            _messages.Add(message);
        }

        public IReadOnlyList<string> Messages => _messages.AsReadOnly();
    }
}