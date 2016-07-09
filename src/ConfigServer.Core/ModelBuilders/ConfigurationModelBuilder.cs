using System;
using System.Linq.Expressions;

namespace ConfigServer.Core
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
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, byte>> expression) => CreateForInterger(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, sbyte>> expression) => CreateForInterger(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, int>> expression) => CreateForInterger(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, uint>> expression) => CreateForInterger(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, long>> expression) => CreateForInterger(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public ConfigurationIntegerPropertyBuilder Property(Expression<Func<TConfig, ulong>> expression) => CreateForInterger(expression);
        #endregion

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, float>> expression) => CreateForFloat(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, double>> expression) => CreateForFloat(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public ConfigurationFloatPropertyBuilder Property(Expression<Func<TConfig, decimal>> expression) => CreateForFloat(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for bool value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationBoolPropertyBuilder for selected property</returns>
        public ConfigurationBoolPropertyBuilder Property(Expression<Func<TConfig, bool>> expression) => new ConfigurationBoolPropertyBuilder(GetOrAddPropertyDefinition(expression));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for string value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationStringPropertyBuilder for selected property</returns>
        public ConfigurationStringPropertyBuilder Property(Expression<Func<TConfig, string>> expression) => new ConfigurationStringPropertyBuilder(GetOrAddPropertyDefinition(expression));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for date time value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationDateTimePropertyBuilder for selected property</returns>
        public ConfigurationDateTimePropertyBuilder Property(Expression<Func<TConfig, DateTime>> expression) => new ConfigurationDateTimePropertyBuilder(GetOrAddPropertyDefinition(expression));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for enum value
        /// </summary>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationEnumPropertyBuilder for selected property</returns>
        public ConfigurationEnumPropertyBuilder Property(Expression<Func<TConfig, Enum>> expression) => new ConfigurationEnumPropertyBuilder(GetOrAddPropertyDefinition(expression));


        private ConfigurationIntegerPropertyBuilder CreateForInterger(LambdaExpression expression)
        {
            return new ConfigurationIntegerPropertyBuilder(GetOrAddPropertyDefinition(expression));
        }

        private ConfigurationFloatPropertyBuilder CreateForFloat(LambdaExpression expression)
        {
            return new ConfigurationFloatPropertyBuilder(GetOrAddPropertyDefinition(expression));
        }

        private ConfigurationPropertyModel GetOrAddPropertyDefinition(LambdaExpression expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }
            ConfigurationPropertyModel value;
            if (!definition.ConfigurationProperties.TryGetValue(body.Member.Name, out value))
            {
                value = new ConfigurationPropertyModel(body.Member.Name);
                definition.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }

            return value;
        }
    }
}
