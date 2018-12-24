using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Attribute used to flag a property is a configuration property and should be added to the configuration model.
    /// Only one is permitted for each property
    /// </summary>
    public abstract class ConfigurationPropertyAttribute : Attribute
    {

    }
}
