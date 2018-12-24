using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{


    internal class ConfigurationPropertyWithMultipleOptionsModelDefinition<TConfigSet, TOption, TOptionCollection> : ConfigurationPropertyWithMultipleOptionsModelDefinition where TOptionCollection : ICollection<TOption> where TConfigSet : ConfigurationSet
    {
        readonly IConfigurationSetOptionProvider<TConfigSet, TOption> optionProvider;
        readonly ConfigurationDependency[] dependency;

        public ConfigurationPropertyWithMultipleOptionsModelDefinition(IConfigurationSetOptionProvider<TConfigSet, TOption> optionProvider, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigSet), typeof(TOption), propertyParentType)
        {
            this.optionProvider = optionProvider;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigSet), optionProvider.OptionPropertyName) };
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TOption>(typeof(TOptionCollection));

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigSet)configurationSet;
            return optionProvider.GetOptions(castedConfigurationSet);
        }

    }

    internal abstract class ConfigurationPropertyWithMultipleOptionsModelDefinition : ConfigurationPropertyWithOptionModelDefinition, IMultipleOptionPropertyDefinition
    {
        protected ConfigurationPropertyWithMultipleOptionsModelDefinition(string propertyName, Type configurationSet, Type optionType, Type propertyParentType) : base(propertyName, configurationSet, optionType, propertyParentType)
        {

        }

        public abstract CollectionBuilder GetCollectionBuilder();

    }
}
