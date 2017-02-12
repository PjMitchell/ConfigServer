using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{

    internal class ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption, TOptionProvider> : ConfigurationPropertyWithMultipleOptionsModelDefinition where TOptionProvider : class where TOptionCollection : ICollection<TOption> where TOption : new ()
    {
        readonly Func<TOptionProvider, IEnumerable<TOption>> optionProvider;
        readonly Func<TOption, string> keySelector;
        readonly Func<TOption, string> displaySelector;

        internal ConfigurationPropertyWithMultipleOptionsModelDefinition(Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector, string propertyName, Type propertyParentType) : base(propertyName, typeof(TOption), propertyParentType)
        {
            this.displaySelector = displaySelector;
            this.keySelector = keySelector;
            this.optionProvider = optionProvider;
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TOption>(typeof(TOptionCollection));

        public override string GetKeyFromObject(object option) => keySelector((TOption)option);
        
        public override IOptionSet BuildOptionSet(IServiceProvider serviceProvider, ConfigurationIdentity configIdentity)
        {
            return new OptionSet<TOption>(GetOptions(serviceProvider), keySelector, displaySelector);
        }

        private IEnumerable<TOption> GetOptions(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService(typeof(TOptionProvider)) as TOptionProvider;
            return optionProvider(provider);
        }
    }

    internal class ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption> : ConfigurationPropertyWithMultipleOptionsModelDefinition where TOptionCollection : ICollection<TOption> where TOption : new()
    {
        readonly Func<IEnumerable<TOption>> optionProvider;
        readonly Func<TOption, string> keySelector;
        readonly Func<TOption, string> displaySelector;

        internal ConfigurationPropertyWithMultipleOptionsModelDefinition(Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector, string propertyName, Type propertyParentType) : base(propertyName, typeof(TOption), propertyParentType)
        {
            this.displaySelector = displaySelector;
            this.keySelector = keySelector;
            this.optionProvider = optionProvider;
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TOption>(typeof(TOptionCollection));

        public override string GetKeyFromObject(object option) => keySelector((TOption)option);

        public override IOptionSet BuildOptionSet(IServiceProvider serviceProvider, ConfigurationIdentity configIdentity)
        {
            return new OptionSet<TOption>(optionProvider(), keySelector, displaySelector);
        }
    }

    internal abstract class ConfigurationPropertyWithMultipleOptionsModelDefinition : ConfigurationPropertyWithOptionsModelDefinition
    {
        protected ConfigurationPropertyWithMultipleOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType) : base(propertyName, propertyType, propertyParentType, true)
        {

        }
        
        public abstract CollectionBuilder GetCollectionBuilder();

    }
}
