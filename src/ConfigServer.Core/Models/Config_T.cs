using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a configuration with meta data such as client id
    /// </summary>
    /// <typeparam name="TConfig">Configuration type</typeparam>
    public class Config<TConfig> : Config where TConfig : class, new()
    {
        /// <summary>
        /// Initializes Config with empty configuration
        /// </summary>
        public Config() : base(typeof(TConfig))
        {
            Configuration = new TConfig();
        }

        /// <summary>
        /// Initializes Config with supplied configuration
        /// </summary>
        /// <param name="config">configuration</param>
        public Config(TConfig config) : base(typeof(TConfig))
        {
            Configuration = config;
        }

        /// <summary>
        /// Configuration object for Config
        /// </summary>
        public TConfig Configuration { get; set; }

        /// <summary>
        /// Gets configuration as object
        /// </summary>
        /// <returns>configuration as object</returns>
        public override object GetConfiguration() => Configuration;

        /// <summary>
        /// Sets configuration
        /// </summary>
        /// <param name="value">value of configuration</param>
        /// <exception cref="InvalidCastException">When object is not of the same type as generic type parameter.</exception>
        public override void SetConfiguration(object value) => Configuration = (TConfig)value;

    }
}
