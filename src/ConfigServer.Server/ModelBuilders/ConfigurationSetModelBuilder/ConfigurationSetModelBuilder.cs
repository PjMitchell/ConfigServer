using ConfigServer.Core;
using System;
using System.Collections.Generic;
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
        /// <typeparam name="TConfiguration">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Config<TConfiguration>(Expression<Func<TConfigurationSet, Config<TConfiguration>>> expression,string displayName, string description) where TConfiguration : class, new()
        {
            var model = definition.GetOrInitialize(expression);
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TConfiguration, TConfigurationSet>(model);
        }

#region Option From OptionSet


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
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Expression<Func<TConfiguration, int>> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName, string description) where TConfiguration : class, new()
        {
            var keyProperty = ExpressionHelper.GetPropertyInfo(keySelector).Name;
            var intKeySelector = keySelector.Compile();
            return OptionsInternal(expression, option => intKeySelector(option).ToString(), descriptionSelector, displayName, description, keyProperty);
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
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Expression<Func<TConfiguration, string>> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName, string description) where TConfiguration : class, new()
        {
            var keyProperty = ExpressionHelper.GetPropertyInfo(keySelector).Name;
            return OptionsInternal(expression, keySelector.Compile(), descriptionSelector, displayName, description, keyProperty);
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
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Expression<Func<TConfiguration, long>> keySelector, Func<TConfiguration, object> descriptionSelector, string displayName, string description) where TConfiguration : class, new()
        {
            var keyProperty = ExpressionHelper.GetPropertyInfo(keySelector).Name;
            var longKeySelector = keySelector.Compile();
            return OptionsInternal(expression, option => longKeySelector(option).ToString(), descriptionSelector, displayName, description, keyProperty);
        }
        #endregion

#region Option From Func




        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector,ci => optionProvider(), displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, keySelector, descriptionSelector, ci => optionProvider(), displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider) where TConfiguration : class, new()
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, typeof(TConfiguration).Name, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider, string displayName) where TConfiguration : class, new()
        {
            return Options(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, string.Empty);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector, ci => optionProvider(), displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, description);
        }


        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, keySelector, descriptionSelector, optionProvider, displayName, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for Options
        /// </summary>
        /// <typeparam name="TConfiguration">Option Type</typeparam>
        /// <param name="expression">Path to Options</param>
        /// <param name="keySelector">Option Key Selector</param>
        /// <param name="descriptionSelector">Option Description Selector</param>
        /// <param name="optionProvider">Option provider</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, description);
        }
#endregion

#region Option From Provider

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
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector,(TOptionProvider op, ConfigurationIdentity i) => optionProvider(op), displayName, description);
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
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, keySelector, descriptionSelector, (TOptionProvider op, ConfigurationIdentity i) => optionProvider(op), displayName, description);
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
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector, (TOptionProvider op, ConfigurationIdentity i) => optionProvider(op), displayName, description);
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
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, int> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, description);
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
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, string> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, keySelector, descriptionSelector, optionProvider, displayName, description);
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
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for Options</returns>
        public ConfigurationModelBuilder<TConfiguration, TConfigurationSet> Options<TConfiguration, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TConfiguration>>> expression, Func<TConfiguration, long> keySelector, Func<TConfiguration, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TConfiguration>> optionProvider, string displayName, string description) where TConfiguration : class, new()
        {
            return ReadOptionsInternal(expression, option => keySelector(option).ToString(), descriptionSelector, optionProvider, displayName, description);
        }
#endregion

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

        private ConfigurationModelBuilder<TOption, TConfigurationSet> OptionsInternal<TOption>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionSelector, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, string displayName, string description, string keyProperty) where TOption : class, new()
        {

            var model = definition.GetOrInitializeOption(typeof(TOption), ()=> BuildOptionModel(optionSelector, keySelector, descriptionSelector));
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            model.KeyPropertyName = keyProperty;
            return new ConfigurationModelBuilder<TOption, TConfigurationSet>(model);
        }

        private ConfigurationOptionModel BuildOptionModel<TOption>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionSelector, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector) where TOption : class, new()
        {
            var propertyInfo = ExpressionHelper.GetPropertyInfo(optionSelector);
            var name = propertyInfo.Name;
            var type = typeof(TOption);
            var setter = (Action<TConfigurationSet, OptionSet<TOption>>)propertyInfo.SetMethod.CreateDelegate(typeof(Action<TConfigurationSet, OptionSet<TOption>>));
            var definition = new ConfigurationOptionModel<TOption, TConfigurationSet>(name, keySelector, descriptionSelector, optionSelector.Compile(), setter);

            ApplyDefaultPropertyDefinitions(definition);

            return definition;
        }

        private ConfigurationModelBuilder<TOption, TConfigurationSet> ReadOptionsInternal<TOption>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionSelector, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TOption>> optionProvider, string displayName, string description) where TOption : class, new()
        {

            var model = definition.GetOrInitializeOption(typeof(TOption), () => BuildReadOptionModel(optionSelector, keySelector, descriptionSelector, optionProvider));
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TOption, TConfigurationSet>(model);
        }

        private ReadOnlyConfigurationOptionModel BuildReadOptionModel<TOption>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionSelector, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<ConfigurationIdentity, IEnumerable<TOption>> optionProvider) where TOption : class, new()
        {
            var propertyInfo = ExpressionHelper.GetPropertyInfo(optionSelector);
            var name = propertyInfo.Name;
            var type = typeof(TOption);
            var setter = (Action<TConfigurationSet, OptionSet<TOption>>)propertyInfo.SetMethod.CreateDelegate(typeof(Action<TConfigurationSet, OptionSet<TOption>>));
            var definition = new ReadOnlyConfigurationOptionModel<TOption, TConfigurationSet>(name, keySelector, descriptionSelector, optionSelector.Compile(), setter, optionProvider);
            ApplyDefaultPropertyDefinitions(definition);

            return definition;
        }

        private ConfigurationModelBuilder<TOption, TConfigurationSet> ReadOptionsInternal<TOption, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionSelector, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TOption>> optionProvider, string displayName, string description) where TOption : class, new()
        {

            var model = definition.GetOrInitializeOption(typeof(TOption), () => BuildReadOptionModel(optionSelector, keySelector, descriptionSelector, optionProvider));
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TOption, TConfigurationSet>(model);
        }

        private ReadOnlyConfigurationOptionModel BuildReadOptionModel<TOption, TOptionProvider>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionSelector, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<TOptionProvider,ConfigurationIdentity, IEnumerable<TOption>> optionProvider) where TOption : class, new()
        {
            var propertyInfo = ExpressionHelper.GetPropertyInfo(optionSelector);
            var name = propertyInfo.Name;
            var type = typeof(TOption);
            var setter = (Action<TConfigurationSet, OptionSet<TOption>>)propertyInfo.SetMethod.CreateDelegate(typeof(Action<TConfigurationSet, OptionSet<TOption>>));
            var definition = new ReadOnlyConfigurationOptionModel<TOption, TConfigurationSet, TOptionProvider>(name, keySelector, descriptionSelector, optionSelector.Compile(), setter, optionProvider);
            ApplyDefaultPropertyDefinitions(definition);

            return definition;
        }

        private void ApplyDefaultPropertyDefinitions(ConfigurationModel model)
        {
            foreach (var kvp in ConfigurationPropertyModelDefinitionFactory.GetDefaultConfigProperties(model.Type))
            {
                model.ConfigurationProperties.Add(kvp.Key, kvp.Value);
            }
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
