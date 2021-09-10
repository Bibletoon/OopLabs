using System;
using System.Collections.Generic;
using System.Linq;

namespace Shops.Tools
{
    public static class NullableExtensions
    {
        public static T ThrowIfNull<T, TException>(this T? argument, TException exception)
            where TException : Exception =>
            argument is null ? throw exception : argument;

        public static List<T> ThrowIfContainsNull<T, TException>(this List<T?> collection, TException exception)
            where TException : Exception
        {
            if (collection.Any(el => el is null))
                throw exception;

            return collection!;
        }
    }
}