using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extension to add Config to ConfigSetModel
    /// </summary>
    public static class ConfigurationSetModelBuilderConfigExtensions
    {
        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to config</param> 
        /// <typeparam name="TConfiguration">Configuration type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Config<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, Config<TConfiguration>>> expression)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Config(expression, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfiguration">Configuration type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Config<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, Config<TConfiguration>>> expression, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Config(expression, displayName, string.Empty);
        }
    }
}
