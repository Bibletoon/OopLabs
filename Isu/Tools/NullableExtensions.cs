using System;

namespace Isu.Tools
{
    public static class NullableExtensions
    {
        public static T ThrowIfNull<T, TException>(this T? argument, TException exception)
            where TException : Exception =>
            argument is null ? throw exception : argument;
    }
}