using System;
using System.Collections;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal abstract class ConfigurationOptionModel : ConfigurationModel
    {
        public ConfigurationOptionModel(string name, Type type, Type configurationSetType) : base(name, type, configurationSetType)
        {
        }

        public abstract IOptionSet BuildOptionSet(IEnumerable souce);
        public Type StoredType { get; protected set; }

    }

    internal class ConfigurationOptionModel<TOption, TConfigurationSet> : ConfigurationOptionModel where TConfigurationSet : ConfigurationSet
    {
        private readonly Func<TOption, string> keySelector;
        private readonly Func<TOption, object> descriptionSelector;
        private readonly Func<TConfigurationSet, TOption> optionSelector;

        public ConfigurationOptionModel(string name, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector) : base(name, typeof(TOption), typeof(TConfigurationSet))
        {
            this.keySelector = keySelector;
            this.descriptionSelector = descriptionSelector;
            StoredType = typeof(List<TOption>);
        }

        private string DescriptionSelector(TOption option)
        {
            return descriptionSelector(option).ToString();
        }

        public override IOptionSet BuildOptionSet(IEnumerable source)
        {
            return new OptionSet<TOption>(source, keySelector, DescriptionSelector);
        }

        public override object GetConfigurationFromConfigurationSet(object configurationSet)
        {
            return optionSelector((TConfigurationSet)configurationSet);
        }
    }
}
