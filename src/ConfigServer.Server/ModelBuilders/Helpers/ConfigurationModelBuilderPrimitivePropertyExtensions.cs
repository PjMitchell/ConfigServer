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
        public static ConfigurationIntegerPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, byte>> expression) => source.CreateForInterger(expression, typeof(byte));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, sbyte>> expression) => source.CreateForInterger(expression, typeof(sbyte));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, int>> expression) => source.CreateForInterger(expression, typeof(int));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, uint>> expression) => source.CreateForInterger(expression, typeof(uint));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, long>> expression) => source.CreateForInterger(expression, typeof(long));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for integer value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationIntegerPropertyBuilder for selected property</returns>
        public static ConfigurationIntegerPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ulong>> expression) => source.CreateForInterger(expression, typeof(ulong));
        #endregion

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, float>> expression) => source.CreateForFloat(expression, typeof(float));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, double>> expression) => source.CreateForFloat(expression, typeof(double));

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for float value
        /// </summary>
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <returns>ConfigurationFloatPropertyBuilder for selected property</returns>
        public static ConfigurationFloatPropertyBuilder Property<TModel>(this IModelWithProperties<TModel> source, Expression<Func<TModel, decimal>> expression) => source.CreateForFloat(expression, typeof(decimal));

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

        private static ConfigurationIntegerPropertyBuilder CreateForInterger<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType)
        {
            return new ConfigurationIntegerPropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, propertyType));
        }

        private static ConfigurationFloatPropertyBuilder CreateForFloat<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType)
        {
            return new ConfigurationFloatPropertyBuilder(source.GetOrAddPrimitivePropertyDefinition(expression, propertyType));
        }

        private static ConfigurationPrimitivePropertyModel GetOrAddPrimitivePropertyDefinition<TModel>(this IModelWithProperties<TModel> source, LambdaExpression expression, Type propertyType)
        {
            var body = GetExpressionBody(expression);
            ConfigurationPropertyModelBase value;
            if (!source.ConfigurationProperties.TryGetValue(body.Member.Name, out value))
            {
                value = new ConfigurationPrimitivePropertyModel(body.Member.Name, propertyType, typeof(TModel));
                source.ConfigurationProperties.Add(value.ConfigurationPropertyName, value);
            }

            return (ConfigurationPrimitivePropertyModel)value;
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
    }
}
