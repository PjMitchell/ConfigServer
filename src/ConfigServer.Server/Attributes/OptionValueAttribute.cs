using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Marks property as an option value with supplied option value provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionValueAttribute : ConfigurationPropertyAttribute
    {
        /// <summary>
        /// Constructor for OptionValueProviderAttribute
        /// </summary>
        /// <param name="optionValueProviderType">Type of option value provider to use for property. Must implement IConfigurationSetOptionValueProvider&lt;TConfigurationSet,TOption,TOptionValue&gt; and have a parameterless constructor</param>
        public OptionValueAttribute(Type optionValueProviderType)
        {
            OptionValueProviderType = optionValueProviderType;
        }

        /// <summary>
        /// Type of option value provider for property
        /// </summary>
        public Type OptionValueProviderType { get; }
    }
}
