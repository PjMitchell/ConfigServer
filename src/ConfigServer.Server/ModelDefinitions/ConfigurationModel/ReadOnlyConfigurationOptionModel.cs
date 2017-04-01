using ConfigServer.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal abstract class ReadOnlyConfigurationOptionModel : ConfigurationOptionModel
    {
        public ReadOnlyConfigurationOptionModel(string name, Type type, Type configurationSetType) : base(name, type, configurationSetType)
        {
            IsReadOnly = true;
        }
        public Type OptionProviderType { get; protected set; }
        public abstract IOptionSet BuildOptionSet(ConfigurationIdentity identity, object optionSource);
    }

    internal class ReadOnlyConfigurationOptionModel<TOption, TConfigurationSet> : ReadOnlyConfigurationOptionModel where TConfigurationSet : ConfigurationSet where TOption : class, new()
    {
        private readonly Func<TOption, string> keySelector;
        private readonly Func<TOption, object> descriptionSelector;
        private readonly Func<ConfigurationIdentity, IEnumerable<TOption>> optionProvider;
        private readonly Func<TConfigurationSet, OptionSet<TOption>> optionSelector;
        private readonly Action<TConfigurationSet, OptionSet<TOption>> configSetter;

        public ReadOnlyConfigurationOptionModel(string name, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<TConfigurationSet, OptionSet<TOption>> optionSelector, Action<TConfigurationSet, OptionSet<TOption>> configSetter, Func<ConfigurationIdentity, IEnumerable<TOption>> optionProvider) : base(name, typeof(TOption), typeof(TConfigurationSet))
        {
            this.keySelector = keySelector;
            this.descriptionSelector = descriptionSelector;
            this.optionSelector = optionSelector;
            this.configSetter = configSetter;
            this.optionProvider = optionProvider;
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

        public override ConfigInstance GetConfigInstanceFromConfigurationSet(object configurationSet)
        {
            var castConfigurationSet = (TConfigurationSet)configurationSet;
            var config = optionSelector(castConfigurationSet);
            return new ConfigCollectionInstance<TOption>(config, castConfigurationSet.Instance);
        }

        public override void SetConfigurationOnConfigurationSet(object configurationSet, object value)
        {
            configSetter((TConfigurationSet)configurationSet, (OptionSet<TOption>)value);
        }

        public override string GetKeyFromObject(object value) => keySelector((TOption)value);

        public override IOptionSet BuildOptionSet(ConfigurationIdentity identity, object optionSource)
        {
            return BuildOptionSet(optionProvider(identity));
        }

        public override object NewItemInstance() => new TOption();
    }

    internal class ReadOnlyConfigurationOptionModel<TOption, TConfigurationSet, TOptionProvider> : ReadOnlyConfigurationOptionModel where TConfigurationSet : ConfigurationSet where TOption : class, new()
    {
        private readonly Func<TOption, string> keySelector;
        private readonly Func<TOption, object> descriptionSelector;
        private readonly Func<TOptionProvider,ConfigurationIdentity, IEnumerable<TOption>> optionProvider;
        private readonly Func<TConfigurationSet, OptionSet<TOption>> optionSelector;
        private readonly Action<TConfigurationSet, OptionSet<TOption>> configSetter;

        public ReadOnlyConfigurationOptionModel(string name, Func<TOption, string> keySelector, Func<TOption, object> descriptionSelector, Func<TConfigurationSet, OptionSet<TOption>> optionSelector, Action<TConfigurationSet, OptionSet<TOption>> configSetter, Func<TOptionProvider, ConfigurationIdentity, IEnumerable<TOption>> optionProvider) : base(name, typeof(TOption), typeof(TConfigurationSet))
        {
            this.keySelector = keySelector;
            this.descriptionSelector = descriptionSelector;
            this.optionSelector = optionSelector;
            this.configSetter = configSetter;
            this.optionProvider = optionProvider;
            OptionProviderType = typeof(TOptionProvider);
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

        public override ConfigInstance GetConfigInstanceFromConfigurationSet(object configurationSet)
        {
            var castConfigurationSet = (TConfigurationSet)configurationSet;
            var config = optionSelector(castConfigurationSet);
            return new ConfigCollectionInstance<TOption>(config, castConfigurationSet.Instance);
        }

        public override void SetConfigurationOnConfigurationSet(object configurationSet, object value)
        {
            configSetter((TConfigurationSet)configurationSet, (OptionSet<TOption>)value);
        }

        public override string GetKeyFromObject(object value) => keySelector((TOption)value);

        public override IOptionSet BuildOptionSet(ConfigurationIdentity identity, object optionSource)
        {
            return BuildOptionSet(optionProvider((TOptionProvider)optionSource, identity));
        }

        public override object NewItemInstance() => new TOption();
    }
}
