using System;

namespace Banks.Tools.Exceptions
{
    public class AccountLimitException : Exception
    {
        public AccountLimitException()
        {
        }

        public AccountLimitException(string message)
            : base(message)
        {
        }

        public AccountLimitException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}