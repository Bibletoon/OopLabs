using System;
using System.Collections.Generic;

namespace BackupsExtra
{
    public class TypeLocator
    {
        private Dictionary<string, Type> _types;

        public TypeLocator()
        {
            _types = new Dictionary<string, Type>();
        }

        public TypeLocator RegisterType<T>()
        {
            _types[typeof(T).FullName] = typeof(T);
            return this;
        }

        public Type GetType(string stringType) => _types[stringType];
    }
}