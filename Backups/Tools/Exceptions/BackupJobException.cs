using System;

namespace Backups.Tools.Exceptions
{
    public class BackupJobException : Exception
    {
        public BackupJobException()
            : base()
        {
        }

        public BackupJobException(string message)
            : base(message)
        {
        }

        public BackupJobException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static BackupJobException ServiceIsMissing(string serviceName)
            => new BackupJobException($"Wrong backup job configuration: service ${serviceName} is missing.");

        public static BackupJobException MissingConfigurationParameter(string parameterName)
            => new BackupJobException($"Configuration parameter {parameterName} is missing");
    }
}