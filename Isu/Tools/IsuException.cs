using System;
using System.Runtime.Serialization;

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

        public static void Throw()
        {
            throw new IsuException("Kekw");
        }
    }
}