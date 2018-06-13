using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Marks class for Configuration model builder
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationClassAttribute : Attribute
    {
    }
}
