using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    /// <summary>
    /// Registry of ConfigurationSets
    /// </summary>
    public interface IConfigurationSetRegistry : IEnumerable<ConfigurationSetModel>
    {
        /// <summary>
        /// Adds new configuration set to the registry
        /// </summary>
        /// <param name="model">ConfigurationSetModel to be added to the registry</param>
        /// <returns>returns true if successful or false if registry already contains configuration set type</returns>
        bool AddConfigurationSet(ConfigurationSetModel model);

        ///<summary>
        /// Gets definition for configuration type
        /// </summary>
        /// <typeparam name="TConfig">configuration type to be retrieved</typeparam>
        /// <returns>ConfigurationModel for selected configuration type</returns>
        /// <exception cref="InvalidOperationException">throws if multiple or no configuration of specified type have been registered</exception>
        ConfigurationModel GetConfigDefinition<TConfig>();


        /// <summary>
        /// Gets definition for configuration type
        /// </summary>
        /// <param name="type">configuration type to be retrieved</param>
        /// <returns>ConfigurationModel for selected configuration type</returns>
        /// <exception cref="InvalidOperationException">throws if multiple or no configuration of specified type have been registered</exception>
        ConfigurationModel GetConfigDefinition(Type type);


        /// <summary>
        /// Gets definition for configuration set type
        /// </summary>
        /// <param name="type">configuration set type to be retrieved</param>
        /// <returns>ConfigurationModel for selected configuration set type</returns>
        ConfigurationSetModel GetConfigSetDefinition(Type type);

        /// <summary>
        /// Tries to get definition for configuration set type
        /// </summary>
        /// <param name="type">configuration set type to be retrieved</param>
        /// <param name="result">ConfigurationModel for selected configuration set type</param>
        /// <returns>True if found else false</returns>
        bool TryGetConfigSetDefinition(Type type, out ConfigurationSetModel result);

        /// <summary>
        /// Gets ConfigurationSet for config type
        /// </summary>
        /// <param name="type">configuration type to be queried</param>
        /// <returns>ConfigurationSetModel for Configuration type</returns>
        ConfigurationSetModel GetConfigSetForConfig(Type type);
    }
}
