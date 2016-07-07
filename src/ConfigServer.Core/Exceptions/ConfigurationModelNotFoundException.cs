using System;

namespace ConfigServer.Core
{
    public class ConfigurationModelNotFoundException : Exception
    {
        public ConfigurationModelNotFoundException(Type type) : base($"Could not find Configuration of Type: {type.FullName}")
        {

        }
    }
}
