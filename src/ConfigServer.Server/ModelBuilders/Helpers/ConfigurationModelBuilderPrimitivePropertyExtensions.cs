using ConfigServer.Server.ModelBuilders;
using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Primitive Builders
    /// </summary>
    public static class ConfigurationModelBuilderPrimitivePropertyExtensions
    {
        #region ConfigurationIntegerPropertyBuilder
        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder<byte> Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, byte>> expression) => source.CreateForInterger<TModel,byte>(expression);

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
        #endregion

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
        /// Gets ConfigurationPropertyModelBuilder for enum value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationEnumPropertyBuilder for selected property</returns>
        public static ConfigurationEnumPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, Enum>> expression) => new ConfigurationEnumPropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(Enum)));

        private static ConfigurationIntegerPropertyBuilder<TProperty> CreateForInterger<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty : IComparable
        {
            return new ConfigurationIntegerPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty)));
        }

        private static ConfigurationFloatPropertyBuilder<TProperty> CreateForFloat<TModel, TProperty>(this IModelWithProperties<TModel> source, LambdaExpression expression) where TProperty : IComparable
        {
            return new ConfigurationFloatPropertyBuilder<TProperty>(source.GetOrAddPrimitivePropertyDefinition(expression, typeof(TProperty)));
        }

        private static ConfigurationPrimitivePropertyModel GetOrAddPrimitivePropertyDefinition<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType)
        {
            var name = ExpressionHelper.GetPropertyNameFromExpression(expression);
            ConfigurationPropertyModelBase value;
            if (!source.ConfigurationProperties.TryGetValue(name, out value))
            {
                value = new ConfigurationPrimitivePropertyModel(name, propertyType, typeof(TModel));
                source.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }

            return (ConfigurationPrimitivePropertyModel)value;
        }


    }
}
