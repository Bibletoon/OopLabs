using System;

namespace Isu.Tools
{
    public class IsuException : Exception
    {
        public IsuException()
        {
        }

        public IsuException(string message)
            : base(message)
        {
        }

        public IsuException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public static IsuException GroupLimitReached()
        {
            return new IsuException("Group students limit reached");
        }
    }
}