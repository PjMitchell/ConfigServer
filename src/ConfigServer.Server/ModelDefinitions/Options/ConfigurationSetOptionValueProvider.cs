using System;
using System.Linq.Expressions;

namespace ConfigServer.Server
{
    /// <summary>
    /// Option provider that gets option from configuration set and value from option
    /// </summary>
    public interface IConfigurationSetOptionValueProvider : IConfigurationSetOptionProvider
    {
        /// <summary>
        /// Option value type
        /// </summary>
        Type OptionValueType { get; }
    }

    /// <summary>
    /// Option provider that gets option from configuration set and value from option
    /// </summary>
    /// <typeparam name="TConfigurationSet">Configuration Set that contains Option</typeparam>
    /// <typeparam name="TOption">Option type</typeparam>
    /// <typeparam name="TOptionValue">Option Value type</typeparam>
    public interface IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TOptionValue> : IConfigurationSetOptionValueProvider, IConfigurationSetOptionProvider<TConfigurationSet, TOption> where TConfigurationSet : ConfigurationSet
    {
        /// <summary>
        /// Gets Value from Option
        /// </summary>
        /// <param name="option">Option that contains value</param>
        /// <returns>Value from Option</returns>
        TOptionValue GetOptionValue(TOption option);
    }

    /// <summary>
    /// Option provider that gets option from configuration set and value from option
    /// </summary>
    /// <typeparam name="TConfigurationSet">Configuration Set that contains Option</typeparam>
    /// <typeparam name="TOption">Option type</typeparam>
    /// <typeparam name="TOptionValue">Option Value type</typeparam>
    public class ConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TOptionValue> : ConfigurationSetOptionProvider<TConfigurationSet, TOption>, IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TOptionValue> where TConfigurationSet : ConfigurationSet
    {
        private readonly Func<TOption, TOptionValue> optionValueProvider;

        /// <summary>
        /// Constructs ConfigurationSetOptionValueProvider with path for option provider and path from option to option value
        /// </summary>
        /// <param name="optionProvider">Path to OptionSet on ConfigurationSet</param>
        /// <param name="optionValueProvider">Path to Value on Option</param>
        public ConfigurationSetOptionValueProvider(Expression<Func<TConfigurationSet, OptionSet<TOption>>> optionProvider, Func<TOption, TOptionValue> optionValueProvider) : base(optionProvider)
        {
            this.optionValueProvider = optionValueProvider;
        }

        /// <summary>
        /// Option value type
        /// </summary>
        public Type OptionValueType => typeof(TOptionValue);

        /// <summary>
        /// Gets Value from Option
        /// </summary>
        /// <param name="option">Option that contains value</param>
        /// <returns>Value from Option</returns>
        public TOptionValue GetOptionValue(TOption option) => optionValueProvider(option);


    }
}
