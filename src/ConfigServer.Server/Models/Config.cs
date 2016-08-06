using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Defines Configuration for configuration set
    /// </summary>
    /// <typeparam name="TConfig">Configuration type</typeparam>
    public class Config<TConfig> 
    {
        /// <summary>
        /// Configuration Type
        /// </summary>
        public Type ConfigType => typeof(TConfig);
    }
}
