using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Option provider that gets option from configuration set
    /// </summary>
    public interface IConfigurationSetOptionProvider
    {
        /// <summary>
        /// Option Property Name
        /// </summary>
        string OptionPropertyName { get; }

        /// <summary>
        /// Type of ConfigurationSet where option is found
        /// </summary>
        Type ConfigurationSetType { get; }

        /// <summary>
        /// Type of Option
        /// </summary>
        Type OptionType { get; }
    }


    /// <summary>
    /// Option provider that gets option from configuration set
    /// </summary>
    /// <typeparam name="TConfigurationSet">Source Configuration Set</typeparam>
    /// <typeparam name="TOption">Option Type</typeparam>
    public interface IConfigurationSetOptionProvider<TConfigurationSet, TOption> : IConfigurationSetOptionProvider where TConfigurationSet : ConfigurationSet
    {
        /// <summary>
        /// Gets Option set from Configuration
        /// </summary>
        /// <param name="configurationSet">Target configuration set</param>
        /// <returns>OptionSet from Configuration Set</returns>
        OptionSet<TOption> GetOptions(TConfigurationSet configurationSet);
    }

    /// <summary>
    /// Implementation of IConfigurationSetOptionProvider build from expression in constructor
    /// </summary>
    /// <typeparam name="TConfigurationSet">Source ConfigurationSet</typeparam>
    /// <typeparam name="TOption">Returned option Type</typeparam>
    public class ConfigurationSetOptionProvider<TConfigurationSet, TOption> : IConfigurationSetOptionProvider<TConfigurationSet, TOption> where TConfigurationSet : ConfigurationSet
    {
        private readonly Func<TConfigurationSet, OptionSet<TOption>> getOptionsFunc;

        /// <summary>
        /// Constructs ConfigurationSetOptionProvider based from expression param
        /// </summary>
        /// <param name="getOptionsFunc">Path to OptionSet on ConfigurationSet</param>
        public ConfigurationSetOptionProvider(Expression<Func<TConfigurationSet, OptionSet<TOption>>> getOptionsFunc)
        {
            this.getOptionsFunc = getOptionsFunc.Compile();
            OptionPropertyName = ExpressionHelper.GetPropertyNameFromExpression(getOptionsFunc);
        }

        /// <summary>
        /// Property of Option on Configuration Set
        /// </summary>
        public string OptionPropertyName { get; }

        /// <summary>
        /// Type of ConfigurationSet where option is found
        /// </summary>
        public Type ConfigurationSetType => typeof(TConfigurationSet);

        /// <summary>
        /// Type of Option
        /// </summary>
        public Type OptionType => typeof(TOption);

        /// <summary>
        /// Gets Option from ConfigurationSet
        /// </summary>
        /// <param name="configurationSet">Source ConfigurationSet</param>
        /// <returns>OptionSet from ConfigurationSet</returns>
        public OptionSet<TOption> GetOptions(TConfigurationSet configurationSet) => getOptionsFunc(configurationSet);
    }

    /// <summary>
    /// ConfigurationSet Option Provider
    /// </summary>
    public class ConfigurationSetOptionProvider
    {
        /// <summary>
        /// Creates ConfigurationSetOptionProvider for given expression
        /// </summary>
        /// <typeparam name="TConfigurationSet">Source ConfigurationSet</typeparam>
        /// <typeparam name="TOption">Returned option Type</typeparam>
        /// <param name="getOptionsFunc">Path to OptionSet on Configuration Set</param>
        /// <returns>ConfigurationSetOptionProvider for given expression</returns>
        public static IConfigurationSetOptionProvider<TConfigurationSet, TOption> Create<TConfigurationSet, TOption>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> getOptionsFunc) where TConfigurationSet : ConfigurationSet
        {
            return new ConfigurationSetOptionProvider<TConfigurationSet, TOption>(getOptionsFunc);
        }

        /// <summary>
        /// Creates ConfigurationSetOptionValueProvider for given expression
        /// </summary>
        /// <typeparam name="TConfigurationSet">Source ConfigurationSet</typeparam>
        /// <typeparam name="TOption">Returned option Type</typeparam>
        /// <typeparam name="TValue">Returned option value Type</typeparam>
        /// <param name="getOptionsFunc">Path to OptionSet on Configuration Set</param>
        /// <param name="valueSelector">Path to value on option</param>
        /// <returns>ConfigurationSetOptionValueProvider for given expression</returns>
        public static IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TValue> Create<TConfigurationSet, TOption, TValue>(Expression<Func<TConfigurationSet, OptionSet<TOption>>> getOptionsFunc, Func<TOption, TValue> valueSelector) where TConfigurationSet : ConfigurationSet
        {
            return new ConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TValue>(getOptionsFunc, valueSelector);
        }
    }
}
