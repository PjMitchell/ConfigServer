using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyWithOptionValueModelDefinition<TConfigurationSet, TOption, TValue> : ConfigurationPropertyWithOptionValueModelDefinition where TConfigurationSet : ConfigurationSet
    {
        readonly IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TValue> optionProvider;
        readonly ConfigurationDependency[] dependency;

        public ConfigurationPropertyWithOptionValueModelDefinition(IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TValue> optionProvider, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigurationSet), typeof(TValue), propertyParentType)
        {
            this.optionProvider = optionProvider;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigurationSet), optionProvider.OptionPropertyName) };
        }

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigurationSet)configurationSet;
            return optionProvider.GetOptions(castedConfigurationSet);
        }

        public override void SetPropertyValue(object config, object value)
        {
            object inputValue;
            if (value is TOption option)
                inputValue = optionProvider.GetOptionValue(option);
            else
                inputValue = value;
            base.SetPropertyValue(config, inputValue);
        }
    }

    internal abstract class ConfigurationPropertyWithOptionValueModelDefinition : ConfigurationPropertyModelBase, IOptionPropertyDefinition
    {
        public ConfigurationPropertyWithOptionValueModelDefinition(string propertyName, Type configurationSet, Type optionType, Type propertyParentType) : base(propertyName, optionType, propertyParentType)
        {
            ConfigurationSetType = configurationSet;
        }

        public Type ConfigurationSetType { get; }

        public abstract IOptionSet GetOptionSet(object configurationSet);

    }
}
