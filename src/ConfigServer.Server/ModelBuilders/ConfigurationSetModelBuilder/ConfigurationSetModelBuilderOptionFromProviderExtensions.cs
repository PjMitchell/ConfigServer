using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extension to add Option to ConfigSetModel that use an Option Provider service
    /// </summary>
    public static class ConfigurationSetModelBuilderOptionFromProviderExtensions
    {
        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()

        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, displayName, string.Empty);
        }


        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }


        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, displayName, string.Empty);
        }


        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TOptionProvider">Option provider Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName)
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }
    }
}
