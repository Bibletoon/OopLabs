using System;

namespace BackupsExtra.Tools
{
    public class RestoreException : Exception
    {
        public RestoreException()
        {
        }

        public RestoreException(string message)
            : base(message)
        {
        }

        public RestoreException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}