using System;

namespace Banks.Tools
{
    public interface IDateTimeProvider
    {
        DateTime Now();
    }
}