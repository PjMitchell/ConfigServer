using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ConfigServer.Server
{
    /// <summary>
    /// Collection Property Builder
    /// </summary>
    public static class ConfigurationModelBuilderCollectionExtensions
    {
        /// <summary>
        /// Gets ConfigurationCollectionPropertyBuilder for a collection
        /// </summary>
        /// <typeparam name="TModel">Property parent Type</typeparam>
        /// <typeparam name="TConfig">Type of object in collection</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">collection selector</param>
        /// <returns>ConfigurationCollectionPropertyBuilder for selected property</returns>
        public static ConfigurationCollectionPropertyBuilder<TConfig> Collection<TModel, TConfig>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TConfig>>> expression) where TConfig : new()
        {
            var body = ExpressionHelper.GetExpressionBody(expression);           
            ConfigurationPropertyModelBase value;
            if (!source.ConfigurationProperties.TryGetValue(body.Member.Name, out value) || !(value is ConfigurationCollectionPropertyDefinition<TConfig>))
            {
                var type = body.Type;
                if (type == typeof(ICollection<TConfig>))
                    type = typeof(List<TConfig>);
                var definition = new ConfigurationCollectionPropertyDefinition<TConfig>(body.Member.Name, typeof(TConfig), typeof(TModel), type);
                ApplyDefaultPropertyDefinitions(definition);
                value = definition;
                source.ConfigurationProperties[value.ConfigurationPropertyName] = value;
            }
            var builder = new ConfigurationCollectionPropertyBuilder<TConfig>((ConfigurationCollectionPropertyDefinition)value);
            return builder;
        }

        private static void ApplyDefaultPropertyDefinitions(ConfigurationCollectionPropertyDefinition model)
        {
            foreach (var kvp in ConfigurationPropertyModelDefinitionFactory.GetDefaultConfigProperties(model.PropertyType))
            {
                model.ConfigurationProperties.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
