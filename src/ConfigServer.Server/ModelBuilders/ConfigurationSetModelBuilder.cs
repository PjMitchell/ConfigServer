using ConfigServer.Server.ModelBuilders;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.Server
{
    /// <summary>
    /// Builder for ConfigurationSetModel
    /// </summary>
    public class ConfigurationSetModelBuilder<TConfigurationSet> where TConfigurationSet : ConfigurationSet<TConfigurationSet>
    {
        private readonly ConfigurationSetModel definition;

        internal ConfigurationSetModelBuilder(string name, string description)
        {
            definition = new ConfigurationSetModel(typeof(TConfigurationSet), name, description);
        }

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <param name="expression">Path to config</param> 
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(Expression<Func<TConfigurationSet, Config<TConfig>>> expression) => Config(expression,typeof(TConfig).Name, string.Empty);

        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(Expression<Func<TConfigurationSet, Config<TConfig>>> expression,string displayName) => Config(expression, displayName, string.Empty);


        /// <summary>
        /// Gets ConfigurationModelBuilder for type
        /// </summary>
        /// <typeparam name="TConfig">Configuration type</typeparam>
        /// <param name="expression">Path to config</param>
        /// <param name="displayName">Display name for the config</param>
        /// <param name="description">Description for the config</param>
        /// <returns>ConfigurationModelBuilder for type</returns>
        public ConfigurationModelBuilder<TConfig> Config<TConfig>(Expression<Func<TConfigurationSet, Config<TConfig>>> expression,string displayName, string description)
        {
            var model = definition.GetOrInitialize<TConfig>(ExpressionHelper.GetPropertyNameFromExpression(expression));
            model.ConfigurationDisplayName = displayName;
            model.ConfigurationDescription = description;
            return new ConfigurationModelBuilder<TConfig>(model);
        }

        /// <summary>
        /// Adds configuration to ConfigurationSetModel
        /// </summary>
        /// <param name="name">name of the configuration model on the configurationSet</param>
        /// <param name="type">type of configuration to be added to configuration set</param>
        public void AddConfig(string name, Type type)
        {
            definition.GetOrInitialize(name,type);            
        }

        /// <summary>
        /// Returns ConfigurationSetModel setup by builder
        /// </summary>
        /// <returns>ConfigurationSetModel setup by builder</returns>
        public ConfigurationSetModel Build() 
        {
            return definition;
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
