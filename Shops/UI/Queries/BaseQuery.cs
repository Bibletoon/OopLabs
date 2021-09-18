using System;

namespace Shops.UI.Queries
{
    public class BaseQuery<T> : IQuery<T>
    {
        private readonly Func<T> _func;

        public BaseQuery(Func<T> func)
        {
            _func = func;
        }

        public T Execute()
        {
            return _func();
        }
    }
}