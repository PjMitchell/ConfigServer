using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TConfigurationSet, TOption,TValue, TValueCollection> : ConfigurationPropertyWithMultipleOptionValuesModelDefinition where TValueCollection : ICollection<TValue> where TConfigurationSet : ConfigurationSet
    {
        readonly IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TValue> optionProvider;
        readonly ConfigurationDependency[] dependency;

        public ConfigurationPropertyWithMultipleOptionValuesModelDefinition(IConfigurationSetOptionValueProvider<TConfigurationSet, TOption, TValue> optionProvider, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigurationSet), typeof(TValue), propertyParentType)
        {
            this.optionProvider = optionProvider;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigurationSet), optionProvider.OptionPropertyName) };
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TValue>(typeof(TValueCollection));

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigurationSet)configurationSet;
            return optionProvider.GetOptions(castedConfigurationSet);
        }

        public override object GetValueFromOption(object option) => optionProvider.GetOptionValue((TOption)option);
    }

    internal abstract class ConfigurationPropertyWithMultipleOptionValuesModelDefinition : ConfigurationPropertyWithOptionValueModelDefinition, IMultipleOptionPropertyDefinition
    {
        protected ConfigurationPropertyWithMultipleOptionValuesModelDefinition(string propertyName, Type configurationSet, Type valueType, Type propertyParentType) : base(propertyName, configurationSet, valueType, propertyParentType)
        {

        }

        public abstract CollectionBuilder GetCollectionBuilder();

        public abstract object GetValueFromOption(object option);
    }
}
