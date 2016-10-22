using System;

namespace ConfigServer.Core
{
    /// <summary>
    /// Represents a configuration with meta data such as client id
    /// </summary>
    /// <typeparam name="TConfig">Configuration type</typeparam>
    public class ConfigInstance<TConfig> : ConfigInstance where TConfig : class, new()
    {
        /// <summary>
        /// Initializes ConfigInstance with empty configuration
        /// </summary>
        public ConfigInstance() : base(typeof(TConfig))
        {
            Configuration = new TConfig();
        }

        /// <summary>
        /// Initializes ConfigInstance with supplied configuration
        /// </summary>
        /// <param name="config">configuration</param>
        public ConfigInstance(TConfig config) : base(typeof(TConfig))
        {
            Configuration = config;
        }

        /// <summary>
        /// Configuration object for ConfigInstance
        /// </summary>
        public TConfig Configuration { get; set; }

        /// <summary>
        /// Constructs a new instance of the configuration
        /// </summary>
        /// <returns>New instance of the configuration </returns>
        public override object ConstructNewConfiguration() => new TConfig();

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
