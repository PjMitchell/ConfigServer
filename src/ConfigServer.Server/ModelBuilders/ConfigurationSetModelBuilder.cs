using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.Server
{
    /// <summary>
    /// Builder for ConfigurationSetModel
    /// </summary>
    public class ConfigurationSetModelBuilder<TConfigurationSet> where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
    {
        private readonly ConfigurationSetModel definition;

        internal ConfigurationSetModelBuilder(string name, string description)
        {
            definition = new ConfigurationSetModel(typeof(TConfigurationSet), name, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <param name="expression">Path to config</param> 
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(Expression<Func<TConfigurationSet, Config<TConfig>>> expression) => Config(expression,typeof(TConfig).Name, string.Empty);

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(Expression<Func<TConfigurationSet, Config<TConfig>>> expression,string displayName) => Config(expression, displayName, string.Empty);


        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(Expression<Func<TConfigurationSet, Config<TConfig>>> expression,string displayName, string description)
        {
            var model = definition.GetOrInitialize<TConfig>(ExpressionHelper.GetPropertyNameFromExpression(expression));
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TConfig>(model);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, int> keySelector, Func<TConfig, object> descriptionSelector)
        {
            return Options(expression, keySelector, descriptionSelector, typeof(TConfig).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, int> keySelector, Func<TConfig, object> descriptionSelector, string displayName)
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, int> keySelector, Func<TConfig, object> descriptionSelector, string displayName, string description)
        {
            return Options(ExpressionHelper.GetPropertyNameFromExpression(expression), option => keySelector(option).ToString(), descriptionSelector, displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, string> keySelector, Func<TConfig, object> descriptionSelector)
        {
            return Options(expression, keySelector, descriptionSelector, typeof(TConfig).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, string> keySelector, Func<TConfig, object> descriptionSelector, string displayName)
        {
            return Options(expression, keySelector, descriptionSelector, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, string> keySelector, Func<TConfig, object> descriptionSelector, string displayName, string description)
        {
            return Options(ExpressionHelper.GetPropertyNameFromExpression(expression), keySelector, descriptionSelector, displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, long> keySelector, Func<TConfig, object> descriptionSelector)
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, typeof(TConfig).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, long> keySelector, Func<TConfig, object> descriptionSelector, string displayName)
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfig">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfig> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, long> keySelector, Func<TConfig, object> descriptionSelector, string displayName, string description)
        {
            return Options(ExpressionHelper.GetPropertyNameFromExpression(expression),option => keySelector(option).ToString(), descriptionSelector, displayName, description);
        }

        /// <summary>
        /// Adds configuration to ConfigurationSetModel
        /// </summary>
        /// <param name="name">name of the configuration model on the configurationSet</param>
        /// <param name="type">type of configuration to be added to configuration set</param>
        public void AddConfig(string name, Type type)
        {
            definition.GetOrInitialize(name,type);            
        }

        /// <summary>
        /// Returns ConfigurationSetModel setup by builder
        /// </summary>
        /// <returns>ConfigurationSetModel setup by builder</returns>
        public ConfigurationSetModel Build() 
        {
            return definition;
        }

        private ConfigurationModelBuilder<TOption> Options<TOption>(string optionPropertyName, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, string displayName, string description)
        {
            var model = definition.GetOrInitializeOption(optionPropertyName, keySelector, descriptionSelector);
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TOption>(model);
        }

        private static MemberExpression GetExpressionBody(LambdaExpression expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            return body;
        }
    }
}
