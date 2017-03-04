using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extension methods for Float property
    /// </summary>
    public static class ConfigurationModelBuilderFloatPropertyExtensions
    {
        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder<float> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, float>> expression) => source.CreateForFloat<TModel, float>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder<double> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, double>> expression) => source.CreateForFloat<TModel, double>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder<decimal> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, decimal>> expression) => source.CreateForFloat<TModel, decimal>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder<float> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, float?>> expression) => source.CreateForNullableFloat<TModel, float>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder<double> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, double?>> expression) => source.CreateForNullableFloat<TModel, double>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder<decimal> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, decimal?>> expression) => source.CreateForNullableFloat<TModel, decimal>(expression);

        private static ConfigurationFloatPropertyBuilder<TProperty> CreateForFloat<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty : IComparable
        {
            return new ConfigurationFloatPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty), true));
        }

        private static ConfigurationFloatPropertyBuilder<TProperty> CreateForNullableFloat<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty : struct, IComparable
        {
            return new ConfigurationFloatPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty?),false));
        }

        private static ConfigurationPrimitivePropertyModel GetOrAddPrimitivePropertyDefinition<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType, bool isRequired)
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
