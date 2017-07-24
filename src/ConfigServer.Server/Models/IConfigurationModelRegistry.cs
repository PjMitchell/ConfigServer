using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    /// <summary>
    /// Registry of Configuration Models
    /// </summary>
    public interface IConfigurationModelRegistry : IEnumerable<ConfigurationSetModel>
    {
        /// <summary>
        /// Gets version of configuration
        /// </summary>
        /// <returns>Version of configuration</returns>
        Version GetVersion();

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

        /// <summary>
        /// Get Configuration Registrations
        /// </summary>
        /// <param name="filterOutReadonlyConfigurations">flags if readonly configruations are to be removed</param>
        /// <returns>All Configuration Registrations</returns>
        IEnumerable<ConfigurationRegistration> GetConfigurationRegistrations(bool filterOutReadonlyConfigurations = false);
    }
}
