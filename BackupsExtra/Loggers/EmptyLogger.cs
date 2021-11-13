using Backups.Tools.Logger;

namespace BackupsExtra.Loggers
{
    public class EmptyLogger : ILogger
    {
        public void Log(string message)
        {
        }
    }
}