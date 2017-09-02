using System;
using System.Linq;
using System.Reflection;

namespace NetUtils
{
    public static class TypeExtensions
    {
        public static bool HasProperty(this Type type, string name)
        {
            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Any(p => p.Name == name);
        }
    }
}