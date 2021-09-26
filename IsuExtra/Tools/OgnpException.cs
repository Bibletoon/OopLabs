using System;

namespace IsuExtra.Tools
{
    public class OgnpException : Exception
    {
        public OgnpException()
        {
        }

        public OgnpException(string name)
            : base(name)
        {
        }

        public OgnpException(string name, Exception innerException)
            : base(name, innerException)
        {
        }

        public static OgnpException OgnpLimitReached()
            => new OgnpException("Ognp by student limit reached");

        public static OgnpException StudentOgnpsLimitReached()
            => new OgnpException("Ognp students limit reached");

        public static OgnpException ScheduleIntersectionOnRegistration()
            => new OgnpException("Can't register to ognp due to intersections");
    }
}