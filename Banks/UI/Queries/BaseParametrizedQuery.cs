using System;

namespace Banks.UI.Queries
{
    public class BaseParametrizedQuery<T, TArgument> : IParameterizedQuery<T, TArgument>
    {
        private readonly Func<TArgument, T> _func;

        public BaseParametrizedQuery(Func<TArgument, T> func)
        {
            _func = func;
        }

        public T Execute(TArgument argument)
        {
            return _func(argument);
        }
    }
}