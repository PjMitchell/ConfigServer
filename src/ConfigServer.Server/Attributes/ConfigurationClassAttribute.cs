using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Marks class for Configuration model builder
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationClassAttribute : Attribute
    {
    }
}
