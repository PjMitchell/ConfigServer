using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TConfigSet, TOption,TValue, TValueCollection> : ConfigurationPropertyWithMultipleOptionValuesModelDefinition where TValueCollection : ICollection<TValue>
    {
        readonly Func<TConfigSet, OptionSet<TOption>> optionProvider;
        readonly Func<TOption, TValue> valueSelector;
        readonly string optionPath;
        readonly ConfigurationDependency[] dependency;

        internal ConfigurationPropertyWithMultipleOptionValuesModelDefinition(Func<TConfigSet, OptionSet<TOption>> optionProvider, Func<TOption, TValue> valueSelector, string optionPath, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigSet), typeof(TValue), propertyParentType)
        {
            this.optionProvider = optionProvider;
            this.optionPath = optionPath;
            this.valueSelector = valueSelector;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigSet), optionPath) };
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TValue>(typeof(TValueCollection));

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigSet)configurationSet;
            return optionProvider(castedConfigurationSet);
        }

        public override object GetValueFromOption(object option) => valueSelector((TOption)option);
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
