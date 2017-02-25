using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyWithConfigSetOptionsModelDefinition<TConfigSet, TOption> : ConfigurationPropertyWithConfigSetOptionsModelDefinition
    {
        readonly Func<TConfigSet, OptionSet<TOption>> optionProvider;
        readonly string optionPath;
        readonly ConfigurationDependency[] dependency;

        internal ConfigurationPropertyWithConfigSetOptionsModelDefinition(Func<TConfigSet, OptionSet<TOption>> optionProvider, string optionPath, string propertyName, Type propertyParentType) : base(propertyName, typeof(TConfigSet), typeof(TOption), propertyParentType)
        {
            this.optionProvider = optionProvider;
            this.optionPath = optionPath;
            dependency = new[] { new ConfigurationDependency(typeof(TConfigSet), optionPath) };
        }

        public override IEnumerable<ConfigurationDependency> GetDependencies() => dependency;

        public override IOptionSet GetOptionSet(object configurationSet)
        {
            var castedConfigurationSet = (TConfigSet)configurationSet;
            return optionProvider(castedConfigurationSet);
        }

    }

    internal abstract class ConfigurationPropertyWithConfigSetOptionsModelDefinition : ConfigurationPropertyModelBase, IOptionPropertyDefinition
    {
        public ConfigurationPropertyWithConfigSetOptionsModelDefinition(string propertyName,Type configurationSet, Type optionType, Type propertyParentType) : base(propertyName, optionType, propertyParentType)
        {
            ConfigurationSetType = configurationSet;
        }

        public Type ConfigurationSetType { get; }

        public abstract IOptionSet GetOptionSet(object configurationSet);

    }
}
