using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
   
    /// <summary>
    /// Property Option Builders
    /// </summary>
    public static class ConfigurationModelBuilderOptionsExtensions
    {
        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TModel, TOption, TOptionProvider>(this IModelWithProperties<TModel> source,  Expression<Func<TModel, TOption>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class
        {
            return source.PropertyWithOptions(expression, optionProvider,option => keySelector(option).ToString(), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TModel,TOption, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOption>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class
        {
            var body = GetExpressionBody(expression);
            return source.PropertyWithOptionsInternal(body, optionProvider, keySelector, displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TModel,TOption>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOption>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector)
        {
            return source.PropertyWithOptions(expression, optionProvider, option => keySelector(option).ToString(), displaySelector);
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel,TOption, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class where TOption : new()
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel,TOption, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class where TOption : new()
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel,TOption>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, int> keySelector, Func<TOption, string> displaySelector) where TOption : new()
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithMulitpleOptions<TModel,TOption>(this IModelWithProperties<TModel> source, Expression<Func<TModel, ICollection<TOption>>> expression, Func< IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOption : new()
        {
            return source.PropertyWithMultipleOptionsInternal(expression, optionProvider, option => keySelector(option), displaySelector);
        }

        /// <summary>
        /// Gets ConfigurationPropertyModelBuilder for property with option
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
        public static ConfigurationPropertyWithOptionBuilder PropertyWithOptions<TModel,TOption>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOption>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector)
        {
            var body = GetExpressionBody(expression);
            return source.PropertyWithOptionsInternal(body, optionProvider, keySelector, displaySelector);
        }

        private static ConfigurationPropertyWithOptionBuilder PropertyWithOptionsInternal<TModel,TOption, TOptionProvider>(this IModelWithProperties<TModel> source, MemberExpression expression, Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionProvider : class
        {
            var model = new ConfigurationPropertyWithOptionsModelDefinition<TOption, TOptionProvider>(optionProvider, keySelector, displaySelector, expression.Member.Name, typeof(TModel));
            source.ConfigurationProperties[expression.Member.Name] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }

        private static ConfigurationPropertyWithOptionBuilder PropertyWithOptionsInternal<TModel,TOption>(this IModelWithProperties<TModel> source, MemberExpression expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) 
        {
            var model = new ConfigurationPropertyWithOptionsModelDefinition<TOption>(optionProvider, keySelector, displaySelector, expression.Member.Name, typeof(TModel));
            source.ConfigurationProperties[expression.Member.Name] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }

        private static ConfigurationPropertyWithOptionBuilder PropertyWithMultipleOptionsInternal<TModel, TOption, TOptionCollection>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOptionCollection>> expression, Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionCollection : ICollection<TOption> where TOption : new()
        {
            var body = GetExpressionBody(expression);
            var model = new ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption>(optionProvider, keySelector, displaySelector, body.Member.Name, typeof(TModel));
            source.ConfigurationProperties[body.Member.Name] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
        }

        private static ConfigurationPropertyWithOptionBuilder PropertyWithMultipleOptionsInternal<TModel, TOption, TOptionCollection, TOptionProvider>(this IModelWithProperties<TModel> source, Expression<Func<TModel, TOptionCollection>> expression, Func<TOptionProvider,IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector) where TOptionCollection : ICollection<TOption> where TOptionProvider: class where TOption : new()
        {
            var body = GetExpressionBody(expression);
            var model = new ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption, TOptionProvider>(optionProvider, keySelector, displaySelector, body.Member.Name, typeof(TModel));
            source.ConfigurationProperties[body.Member.Name] = model;
            return new ConfigurationPropertyWithOptionBuilder(model);
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
