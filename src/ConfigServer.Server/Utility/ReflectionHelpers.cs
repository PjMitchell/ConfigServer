using System;
using System.Reflection;

namespace ConfigServer.Server
{
    internal class ReflectionHelpers
    {
        public static Type BuildGenericType(Type genericType, params Type[] typeArgs)
        {
            return genericType.MakeGenericType(typeArgs);
        }
    }
}
