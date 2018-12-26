using System;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    internal static class AttributeHelpers
    {
        public static T GetAttributeFromOrDefault<T>(this Type type, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = type.GetProperty(propertyName);
            return (T)property.GetCustomAttributes(attrType, false).FirstOrDefault();
        }

        public static bool HasAttribute<T>(this PropertyInfo info) where T : Attribute
        {
            return info.GetCustomAttributes<T>().Any();
        }

        public static T SingleAttributeOrDefault<T>(this Type type) where T : Attribute
        {
            return type.GetTypeInfo().GetCustomAttributes<T>().SingleOrDefault();
        }

    }
}
