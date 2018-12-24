using System;

namespace ConfigServer.Server
{
    /// <summary>
    /// Marks property as an option with supplied option provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : ConfigurationPropertyAttribute
    {
        /// <summary>
        /// Constructor for OptionAttribute
        /// </summary>
        /// <param name="optionProviderType">Type of option provider to use for property. Must implement IConfigurationSetOptionProvider&lt;TConfigurationSet,TOption&gt; and have a parameterless constructor </param>
        public OptionAttribute(Type optionProviderType)
        {
            OptionProviderType = optionProviderType;            
        }

        /// <summary>
        /// Type of option provider for property
        /// </summary>
        public Type OptionProviderType { get; }
    }
}
