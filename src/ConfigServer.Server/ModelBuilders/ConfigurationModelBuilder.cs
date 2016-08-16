using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Builder for ConfigurationModel 
    /// </summary>
    /// <typeparam name="TConfig">Configuration type being built</typeparam>
    public class ConfigurationModelBuilder<TConfig>
    {
        private readonly ConfigurationModel definition;

        internal ConfigurationModelBuilder(ConfigurationModel definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Returns ConfigurationModel setup by builder
        /// </summary>
        /// <returns>ConfigurationModel setup by builder</returns>
        public ConfigurationModel Build() => definition;

        #region ConfigurationIntegerPropertyBuilder
        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, byte>> expression) => CreateForInterger(expression, typeof(byte));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, sbyte>> expression) => CreateForInterger(expression, typeof(sbyte));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, int>> expression) => CreateForInterger(expression, typeof(int));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, uint>> expression) => CreateForInterger(expression, typeof(uint));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, long>> expression) => CreateForInterger(expression, typeof(long));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, ulong>> expression) => CreateForInterger(expression, typeof(ulong));
        #endregion

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, float>> expression) => CreateForFloat(expression, typeof(float));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, double>> expression) => CreateForFloat(expression, typeof(double));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, decimal>> expression) => CreateForFloat(expression, typeof(decimal));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for bool value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationBoolPropertyBuilder for selected property</returns>
        public ConfigurationBoolPropertyBuilder Property(Expression<Func<TConfig, bool>> expression) => new ConfigurationBoolPropertyBuilder(GetOrAddPrimitivePropertyDefinition(expression, typeof(bool)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for string value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationStringPropertyBuilder for selected property</returns>
        public ConfigurationStringPropertyBuilder Property(Expression<Func<TConfig, string>> expression) => new ConfigurationStringPropertyBuilder(GetOrAddPrimitivePropertyDefinition(expression, typeof(string)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for date time value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationDateTimePropertyBuilder for selected property</returns>
        public ConfigurationDateTimePropertyBuilder Property(Expression<Func<TConfig, DateTime>> expression) => new ConfigurationDateTimePropertyBuilder(GetOrAddPrimitivePropertyDefinition(expression, typeof(DateTime)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for enum value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationEnumPropertyBuilder for selected property</returns>
        public ConfigurationEnumPropertyBuilder Property(Expression<Func<TConfig, Enum>> expression) => new ConfigurationEnumPropertyBuilder(GetOrAddPrimitivePropertyDefinition(expression,typeof(Enum)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
        /// Overides existing configuration from property
        /// </summary>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <typeparam name="TOptionProvider">Class used to provide available options</typeparam>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TOption, TOptionProvider>(Expression<Func<TConfig, TOption>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class
        {
            return PropertyWithOptions(expression, optionProvider,option => keySelector(option).ToString(), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
        /// Overides existing configuration from property
        /// </summary>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <typeparam name="TOptionProvider">Class used to provide available options</typeparam>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TOption, TOptionProvider>(Expression<Func<TConfig, TOption>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class
        {
            var body = GetExpressionBody(expression);
            var model = new ConfigurationPropertyWithOptionsModelDefinition<TOptionProvider, TOption>(optionProvider, keySelector, displaySelector, body.Member.Name, definition.Type);
            definition.ConfigurationProperties[body.Member.Name] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
        /// Overides existing configuration from property
        /// </summary>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TOption>(Expression<Func<TConfig, TOption>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector)
        {
            return PropertyWithOptions(expression, optionProvider, option => keySelector(option).ToString(), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
        /// Overides existing configuration from property
        /// </summary>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TOption>(Expression<Func<TConfig, TOption>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector)
        {
            var body = GetExpressionBody(expression);
            var model = new ConfigurationPropertyWithOptionsModelDefinition<TOption>(optionProvider, keySelector, displaySelector, body.Member.Name, definition.Type);
            definition.ConfigurationProperties[body.Member.Name] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }

        private ConfigurationIntegerPropertyBuilder CreateForInterger(LambdaExpression expression, Type propertyType)
        {
            return new ConfigurationIntegerPropertyBuilder(GetOrAddPrimitivePropertyDefinition(expression, propertyType));
        }

        private ConfigurationFloatPropertyBuilder CreateForFloat(LambdaExpression expression, Type propertyType)
        {
            return new ConfigurationFloatPropertyBuilder(GetOrAddPrimitivePropertyDefinition(expression, propertyType));
        }

        private ConfigurationPrimitivePropertyModel GetOrAddPrimitivePropertyDefinition(LambdaExpression expression, Type propertyType)
        {
            var body = GetExpressionBody(expression);
            ConfigurationPropertyModelBase value;
            if (!definition.ConfigurationProperties.TryGetValue(body.Member.Name, out value))
            {
                value = new ConfigurationPrimitivePropertyModel(body.Member.Name, propertyType, definition.Type);
                definition.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }

            return (ConfigurationPrimitivePropertyModel)value;
        }

        private MemberExpression GetExpressionBody(LambdaExpression expression)
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
