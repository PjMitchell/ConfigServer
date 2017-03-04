using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{



    /// <summary>
    /// Property Primitive Builders
    /// </summary>
    public static class ConfigurationModelBuilderPrimitivePropertyExtensions
    {

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for bool value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationBoolPropertyBuilder for selected property</returns>
        public static ConfigurationBoolPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, bool>> expression) => new ConfigurationBoolPropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(bool)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for string value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationStringPropertyBuilder for selected property</returns>
        public static ConfigurationStringPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, string>> expression) => new ConfigurationStringPropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(string)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for date time value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationDateTimePropertyBuilder for selected property</returns>
        public static ConfigurationDateTimePropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, DateTime>> expression) => new ConfigurationDateTimePropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(DateTime)));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for a nullable date time value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationDateTimePropertyBuilder for selected property</returns>
        public static ConfigurationDateTimePropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, DateTime?>> expression) => new ConfigurationDateTimePropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(DateTime?), false));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for enum value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationEnumPropertyBuilder for selected property</returns>
        public static ConfigurationEnumPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, Enum>> expression) => new ConfigurationEnumPropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(Enum)));



        private static ConfigurationFloatPropertyBuilder<TProperty> CreateForFloat<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty : IComparable
        {
            return new ConfigurationFloatPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty)));
        }

        private static ConfigurationPrimitivePropertyModel GetOrAddPrimitivePropertyDefinition<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType, bool isRequired = true)
        {
            var name = ExpressionHelper.GetPropertyNameFromExpression(expression);
            ConfigurationPropertyModelBase value;
            if (!source.ConfigurationProperties.TryGetValue(name, out value))
            {
                value = new ConfigurationPrimitivePropertyModel(name, propertyType, typeof(TModel));
                source.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }
            var result = (ConfigurationPrimitivePropertyModel)value;
            result.ValidationRules.IsRequired = isRequired;
            return result;
        }


    }
}
