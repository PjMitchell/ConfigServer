using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.Server
{
    /// <summary>
    /// Builder for ConfigurationSetModel
    /// </summary>
    public class ConfigurationSetModelBuilder<TConfigurationSet> where TConfigurationSet : ConfigurationSet<TConfigurationSet>, new()
    {
        private readonly ConfigurationSetModel<TConfigurationSet> definition;

        internal ConfigurationSetModelBuilder(string name, string description)
        {
            definition = new ConfigurationSetModel<TConfigurationSet>(name, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <param name="expression">Path to config</param> 
        /// <typeparam name="TConfiguration">Configuration type</typeparam>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Config<TConfiguration>(Expression<Func<TConfigurationSet, Config<TConfiguration>>> expression) => Config(expression,typeof(TConfiguration).Name, string.Empty);

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfiguration">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Config<TConfiguration>(Expression<Func<TConfigurationSet, Config<TConfiguration>>> expression,string displayName) => Config(expression, displayName, string.Empty);


        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfiguration">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Config<TConfiguration>(Expression<Func<TConfigurationSet, Config<TConfiguration>>> expression,string displayName, string description)
        {
            var model = definition.GetOrInitialize(ExpressionHelper.GetPropertyNameFromExpression(expression), expression.Compile());
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TConfiguration, TConfigurationSet>(model);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector)
        {
            return Options(expression, keySelector, descriptionSelector, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName)
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName, string description)
        {
            return Options(ExpressionHelper.GetPropertyNameFromExpression(expression), option => keySelector(option).ToString(), descriptionSelector, displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector)
        {
            return Options(expression, keySelector, descriptionSelector, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName)
        {
            return Options(expression, keySelector, descriptionSelector, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName, string description)
        {
            return Options(ExpressionHelper.GetPropertyNameFromExpression(expression), keySelector, descriptionSelector, displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector)
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, typeof(TConfiguration).Name, string.Empty);
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
        public ConfigurationModelBuilder<TConfig, TConfigurationSet> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, long> keySelector, Func<TConfig, object> descriptionSelector, string displayName)
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
        public ConfigurationModelBuilder<TConfig, TConfigurationSet> Options<TConfig>(Expression<Func<TConfigurationSet, OptionSet<TConfig>>> expression, Func<TConfig, long> keySelector, Func<TConfig, object> descriptionSelector, string displayName, string description)
        {
            return Options(ExpressionHelper.GetPropertyNameFromExpression(expression),option => keySelector(option).ToString(), descriptionSelector, displayName, description);
        }

        /// <summary>
        /// Adds configuration to ConfigurationSetModel
        /// </summary>
        /// <param name="configProperty">property info of ConfigurationSet Config Property</param>
        public void AddConfig(PropertyInfo configProperty)
        {
            definition.GetOrInitialize(configProperty);            
        }

        /// <summary>
        /// Returns ConfigurationSetModel setup by builder
        /// </summary>
        /// <returns>ConfigurationSetModel setup by builder</returns>
        public ConfigurationSetModel Build() 
        {
            return definition;
        }

        private ConfigurationModelBuilder<TOption, TConfigurationSet> Options<TOption>(string optionPropertyName, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, string displayName, string description)
        {
            var model = definition.GetOrInitializeOption(optionPropertyName, keySelector, descriptionSelector);
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TOption, TConfigurationSet>(model);
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
