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

    }
}
