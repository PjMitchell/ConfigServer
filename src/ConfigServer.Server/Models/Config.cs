using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Defines Configuration for configuration set
    /// </summary>
    /// <typeparam name="TConfig">Configuration type</typeparam>
    public class Config<TConfig> : Config
    {
        /// <summary>
        /// Configuration Type
        /// </summary>
        public Type ConfigType => typeof(TConfig);

        /// <summary>
        /// Value of config Instance
        /// </summary>
        public TConfig Value { get; set; }

        /// <summary>
        /// Gets configuration as object
        /// </summary>
        /// <returns>configuration as object</returns>
        public override object GetConfig() => Value;

        /// <summary>
        /// Sets configuration
        /// </summary>
        /// <param name="value">value of configuration</param>
        /// <exception cref="InvalidCastException">When object is not of the same type as generic type parameter.</exception>
        public override void SetConfig(object value) => Value = (TConfig)value;
    }

    /// <summary>
    /// Defines Configuration for configuration set
    /// </summary>
    public abstract class Config
    {
        /// <summary>
        /// Sets configuration
        /// </summary>
        /// <param name="value">value of configuration</param>
        /// <exception cref="InvalidCastException">When object is not of the same type as generic type parameter.</exception>
        public abstract void SetConfig(object value);

        /// <summary>
        /// Gets configuration as object
        /// </summary>
        /// <returns>configuration as object</returns>
        public abstract object GetConfig();

        /// <summary>
        /// Creates new instance of a Config
        /// </summary>
        /// <param name="configType">Type of Config</param>
        /// <param name="value">Config value</param>
        /// <returns>Populated config</returns>
        public static Config Create(Type configType, object value)
        {
            var config = typeof(Config<>);
            Type[] typeArgs = { configType };
            var genericType = config.MakeGenericType(typeArgs);
            var result = (Config)Activator.CreateInstance(genericType);
            result.SetConfig(value);
            return result;
        }
    }
}
