using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extension to add Option to ConfigSetModel
    /// </summary>
    public static class ConfigurationSetModelBuilderReadOnlyOptionExtensions
    {
        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider, string displayName) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider, string displayName) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }


        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, keySelector, descriptionSelector, optionProvider, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <typeparam name="TConfigurationSet">ConfigurationSet type</typeparam>
        /// <param name="source">ConfigurationSetModelBuilder</param> 
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public static ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TConfigurationSet>(this ConfigurationSetModelBuilder<TConfigurationSet> source, Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName) 
            where TConfiguration : class, new()
            where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
        {
            return source.Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }

    }
}
