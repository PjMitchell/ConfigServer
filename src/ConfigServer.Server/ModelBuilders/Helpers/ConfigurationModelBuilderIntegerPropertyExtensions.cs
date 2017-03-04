using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extension methods for Integer property
    /// </summary>
    public static class ConfigurationModelBuilderIntegerPropertyExtensions
    {

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<byte> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, byte>> expression) => source.CreateForInterger<TModel, byte>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<sbyte> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, sbyte>> expression) => source.CreateForInterger<TModel, sbyte>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<int> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, int>> expression) => source.CreateForInterger<TModel, int>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<uint> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, uint>> expression) => source.CreateForInterger<TModel, uint>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<long> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, long>> expression) => source.CreateForInterger<TModel, long>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<ulong> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ulong>> expression) => source.CreateForInterger<TModel, ulong>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<byte> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, byte?>> expression) => source.CreateForNullableInterger<TModel, byte>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<sbyte> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, sbyte?>> expression) => source.CreateForNullableInterger<TModel, sbyte>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<int> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, int?>> expression) => source.CreateForNullableInterger<TModel, int>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<uint> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, uint?>> expression) => source.CreateForNullableInterger<TModel, uint>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<long> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, long?>> expression) => source.CreateForNullableInterger<TModel, long>(expression);

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<ulong> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ulong?>> expression) => source.CreateForNullableInterger<TModel, ulong>(expression);

        private static ConfigurationIntegerPropertyBuilder<TProperty> CreateForInterger<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty : IComparable
        {
            return new ConfigurationIntegerPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty), true));
        }

        private static ConfigurationIntegerPropertyBuilder<TProperty> CreateForNullableInterger<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty :struct, IComparable
        {
            return new ConfigurationIntegerPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty?), false));
        }

        private static ConfigurationPrimitivePropertyModel GetOrAddPrimitivePropertyDefinition<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType, bool isRequired)
        {
            var name = ExpressionHelper.GetPropertyNameFromExpression(expression);
            if (!source.ConfigurationProperties.TryGetValue(name, out ConfigurationPropertyModelBase value))
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
