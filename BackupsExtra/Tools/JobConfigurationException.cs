using System;

namespace BackupsExtra.Tools
{
    public class JobConfigurationException : Exception
    {
        public JobConfigurationException()
        {
        }

        public JobConfigurationException(string message)
            : base(message)
        {
        }

        public JobConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}