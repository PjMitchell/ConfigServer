using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{


    internal class ConfigurationPropertyWithMultipleOptionsModelDefinition<TConfigSet, TOption, TOptionCollection> : ConfigurationPropertyWithMultipleOptionsModelDefinition where TOptionCollection : ICollection<TOption> where TOption : new()
    {
        readonly Func<TConfigSet, OptionSet<TOption>> optionProvider;
        readonly string optionPath;
        readonly ConfigurationDependency[] dependency;

        internal ConfigurationPropertyWithMultipleOptionsModelDefinition(Func<TConfigSet, OptionSet<TOption>> optionProvider, string optionPath, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigSet), typeof(TOption), propertyParentType)
        {
            this.optionProvider = optionProvider;
            this.optionPath = optionPath;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigSet), optionPath) };
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TOption>(typeof(TOptionCollection));

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigSet)configurationSet;
            return optionProvider(castedConfigurationSet);
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
