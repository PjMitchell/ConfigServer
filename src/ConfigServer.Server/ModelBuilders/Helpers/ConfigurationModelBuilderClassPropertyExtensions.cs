using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{



    /// <summary>
    /// Property Primitive Builders
    /// </summary>
    public static class ConfigurationModelBuilderClassPropertyExtensions
    {

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for Class value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <typeparam name="TClass">Class model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationBoolPropertyBuilder for selected property</returns>
        public static ConfigurationClassPropertyBuilder<TClass> Property<TModel, TClass>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TClass>> expression) where TClass : new() => new ConfigurationClassPropertyBuilder<TClass>(source.GetOrAddClassPropertyDefinition<TModel, TClass>(expression));

        private static ConfigurationClassPropertyDefinition<TClass> GetOrAddClassPropertyDefinition<TModel, TClass>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TClass : new()
        {
            var name = ExpressionHelper.GetPropertyNameFromExpression(expression);
            ConfigurationPropertyModelBase value;
            if (!source.ConfigurationProperties.TryGetValue(name, out value))
            {
                var definition = new ConfigurationClassPropertyDefinition<TClass>(name, typeof(TClass), typeof(TModel));
                ApplyDefaultPropertyDefinitions(definition);
                value = definition;
                source.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }
            var result = (ConfigurationClassPropertyDefinition<TClass>)value;
            return result;
        }

        private static void ApplyDefaultPropertyDefinitions(ConfigurationClassPropertyDefinition model)
        {
            foreach (var kvp in ConfigurationPropertyModelDefinitionFactory.GetDefaultConfigProperties(model.PropertyType))
            {
                model.ConfigurationProperties.Add(kvp.Key, kvp.Value);
            }
        }
    }
}
