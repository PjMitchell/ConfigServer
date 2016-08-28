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
        public static ConfigurationCollectionPropertyBuilder<TConfig> Collection<TModel, TConfig>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TConfig>>> expression)
        {
            var body = GetExpressionBody(expression);           
            ConfigurationPropertyModelBase value;
            if (!source.ConfigurationProperties.TryGetValue(body.Member.Name, out value))
            {
                value = new ConfigurationCollectionPropertyDefinition(body.Member.Name, typeof(TConfig), typeof(TModel));
                source.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }
            var builder = new ConfigurationCollectionPropertyBuilder<TConfig>((ConfigurationCollectionPropertyDefinition)value);
            return builder;
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

        private static void ApplyDefaultPropertyDefinitions(ConfigurationCollectionPropertyDefinition model)
        {
            foreach (PropertyInfo writeProperty in model.PropertyType.GetProperties().Where(prop => prop.CanWrite))
            {
                model.ConfigurationProperties.Add(writeProperty.Name, ConfigurationPropertyModelDefinitionFactory.Build(writeProperty, model.PropertyType));
            }
        }
    }
}
