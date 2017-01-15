using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Builder for a property that is a collection
    /// </summary>
    /// <typeparam name="TConfig">Type of object in collection</typeparam>
    public class ConfigurationCollectionPropertyBuilder<TConfig> : IModelWithProperties<TConfig>
    {
        private readonly ConfigurationCollectionPropertyDefinition definition;

        internal ConfigurationCollectionPropertyBuilder(ConfigurationCollectionPropertyDefinition definition)
        {
            this.definition = definition;
        }

        /// <summary>
        /// Propertes of collection object
        /// </summary>
        public Dictionary<string, ConfigurationPropertyModelBase> ConfigurationProperties => definition.ConfigurationProperties;

        /// <summary>
        /// Declares a Unique key for Collection model
        /// </summary>
        /// <param name="expression">Path to Unique key</param>
        /// <returns>Builder</returns>
        public ConfigurationCollectionPropertyBuilder<TConfig> WithUniqueKey(Expression<Func<TConfig, int>> expression) => WithUniqueKeyInternal(expression);

        /// <summary>
        /// Declares a Unique key for Collection model
        /// </summary>
        /// <param name="expression">Path to Unique key</param>
        /// <returns>Builder</returns>
        public ConfigurationCollectionPropertyBuilder<TConfig> WithUniqueKey(Expression<Func<TConfig, long>> expression) => WithUniqueKeyInternal(expression);

        /// <summary>
        /// Declares a Unique key for Collection model
        /// </summary>
        /// <param name="expression">Path to Unique key</param>
        /// <returns>Builder</returns>
        public ConfigurationCollectionPropertyBuilder<TConfig> WithUniqueKey(Expression<Func<TConfig, string>> expression) => WithUniqueKeyInternal(expression);

        private ConfigurationCollectionPropertyBuilder<TConfig> WithUniqueKeyInternal(LambdaExpression expression)
        {
            var propertyName = ExpressionHelper.GetPropertyNameFromExpression(expression);
            definition.HasUniqueKey = true;
            definition.KeyPropertyName = propertyName;
            return this;
        }
    }
}
