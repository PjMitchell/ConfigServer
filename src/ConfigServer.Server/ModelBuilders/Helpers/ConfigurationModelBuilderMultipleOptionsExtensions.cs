using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Property Multiple Option Builders
    /// </summary>
    public static class ConfigurationModelBuilderMultipleOptionsExtensions
    {
        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with multiple option
        /// Overides existing configuration from property
        /// </summary>       
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <typeparam name="TOptionProvider">Class used to provide available options</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel, TOption, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class where TOption : new()
        {
            return source.PropertyWithMulitpleOptions(expression, optionProvider, option => keySelector(option).ToString(), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with multiple option
        /// Overides existing configuration from property
        /// </summary>       
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <typeparam name="TOptionProvider">Class used to provide available options</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel, TOption, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class where TOption : new()
        {
            return source.PropertyWithMultipleOptionsInternal(expression, optionProvider, option => keySelector(option), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with multiple option
        /// Overides existing configuration from property
        /// </summary>       
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel, TOption>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector) where TOption : new()
        {
            return source.PropertyWithMulitpleOptions(expression, optionProvider, option => keySelector(option).ToString(), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with multiple option
        /// Overides existing configuration from property
        /// </summary>       
        /// <typeparam name="TModel">Source model type</typeparam>
        /// <typeparam name="TOption">Option type</typeparam>
        /// <param name="source">model with property</param>
        /// <param name="expression">property selector</param>
        /// <param name="optionProvider">Function that provides the available options</param>
        /// <param name="keySelector">Selector for the option key</param>
        /// <param name="displaySelector">Selector for the option display value</param>
        /// <returns>ConfigurationPropertyWithOptionBuilder for selected property</returns>
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel, TOption>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOption : new()
        {
            return source.PropertyWithMultipleOptionsInternal(expression, optionProvider, option => keySelector(option), displaySelector);
        }

        private static ConfigurationPropertyWithOptionBuilder PropertyWithMultipleOptionsInternal<TModel, TOption, TOptionCollection>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOptionCollection>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionCollection : ICollection<TOption> where TOption : new()
        {
            var propertyName = ExpressionHelper.GetPropertyNameFromExpression(expression);
            var model = new ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption>(optionProvider, keySelector, displaySelector, propertyName, typeof(TModel));
            source.ConfigurationProperties[propertyName] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }

        private static ConfigurationPropertyWithOptionBuilder PropertyWithMultipleOptionsInternal<TModel, TOption, TOptionCollection, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOptionCollection>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionCollection : ICollection<TOption> where TOptionProvider : class where TOption : new()
        {
            var propertyName = ExpressionHelper.GetPropertyNameFromExpression(expression);
            var model = new ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption, TOptionProvider>(optionProvider, keySelector, displaySelector, propertyName, typeof(TModel));
            source.ConfigurationProperties[propertyName] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }
    }
}
