using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        public override IEnumerable<ConfigurationPropertyOptionDefintion> GetAvailableOptions(IServiceProvider serviceProvider)
        {
            return GetOptions(serviceProvider).Select(s => new ConfigurationPropertyOptionDefintion { Key = keySelector(s), DisplayValue = displaySelector(s) });
        }

        public override bool TryGetOption(IServiceProvider serviceProvider, string key, out object option)
        {
            option = GetOptions(serviceProvider).SingleOrDefault(s => keySelector(s) == key);
            return option != null;
        }

        private IEnumerable<TOption> GetOptions(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService(typeof(TOptionProvider)) as TOptionProvider;
            return optionProvider(provider);
        }

        public override bool OptionMatchesKey(string key, object option)
        {
            if (option is TOptionCollection)
                return ((TOptionCollection)option).Any(a => keySelector(a) == key);
            return false;
        }
        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TOption>(typeof(TOptionCollection));
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

        public override IEnumerable<ConfigurationPropertyOptionDefintion> GetAvailableOptions(IServiceProvider serviceProvider)
        {
            return GetOptions().Select(s => new ConfigurationPropertyOptionDefintion { Key = keySelector(s), DisplayValue = displaySelector(s) });
        }

        public override bool TryGetOption(IServiceProvider serviceProvider, string key, out object option)
        {
            option = GetOptions().SingleOrDefault(s => keySelector(s) == key);
            return option != null;
        }

        private IEnumerable<TOption> GetOptions()
        {
            return optionProvider();
        }

        public override bool OptionMatchesKey(string key, object option)
        {
            if (option is TOptionCollection)
                return ((TOptionCollection)option).Any(a => keySelector(a) == key);
            return false;
        }

        public override CollectionBuilder GetCollectionBuilder() => new CollectionBuilder<TOption>(typeof(TOptionCollection));

    }

    internal abstract class ConfigurationPropertyWithMultipleOptionsModelDefinition : ConfigurationPropertyWithOptionsModelDefinition
    {
        protected ConfigurationPropertyWithMultipleOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType) : base(propertyName, propertyType, propertyParentType, true)
        {

        }

        //public abstract void SetPropertyValue(IServiceProvider serviceProvider, object config, IEnumerable<string> options);

        /// <summary>
        /// Gets Collection Builder for Property
        /// </summary>
        /// <returns>Collection Builder for Property</returns>
        public abstract CollectionBuilder GetCollectionBuilder();

    }
}
