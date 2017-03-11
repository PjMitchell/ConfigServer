using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyWithOptionValueModelDefinition<TConfigSet, TOption, TValue> : ConfigurationPropertyWithOptionModelDefinition
    {
        readonly Func<TConfigSet, OptionSet<TOption>> optionProvider;
        readonly Func<TOption, TValue> valueSelector;

        readonly string optionPath;
        readonly ConfigurationDependency[] dependency;

        internal ConfigurationPropertyWithOptionValueModelDefinition(Func<TConfigSet, OptionSet<TOption>> optionProvider, Func<TOption, TValue> valueSelector, string optionPath, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigSet), typeof(TValue), propertyParentType)
        {
            this.optionProvider = optionProvider;
            this.valueSelector = valueSelector;
            this.optionPath = optionPath;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigSet), optionPath) };
        }

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigSet)configurationSet;
            return optionProvider(castedConfigurationSet);
        }

        public override void SetPropertyValue(object config, object value)
        {
            object inputValue;
            if (value is TOption option)
                inputValue = valueSelector(option);
            else
                inputValue = value;
            base.SetPropertyValue(config, inputValue);
        }
    }
}
