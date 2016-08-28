using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{

    internal class ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption, TOptionProvider> : ConfigurationPropertyWithMultipleOptionsModelDefinition where TOptionProvider : class where TOptionCollection : ICollection<TOption>
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

        public override void SetPropertyValue(IServiceProvider serviceProvider, object config, IEnumerable<string> options)
        {
            var optionLookup = GetAvailableOptions(serviceProvider).ToDictionary(k => k);
            var emptyCollection = CreateNew();
            foreach (var key in options)
            {
                object option;
                if (TryGetOption(serviceProvider, key, out option))
                    emptyCollection.Add((TOption)option);
            }
            SetPropertyValue(config, emptyCollection);
        }


        protected ICollection<TOption> CreateNew()
        {
            if (typeof(TOptionCollection) == typeof(ICollection<TOption>))
                return new List<TOption>();
            var constructor = typeof(TOptionCollection).GetConstructor(Type.EmptyTypes);
            return (TOptionCollection)constructor.Invoke(new object[0]);
        }
    }

    internal class ConfigurationPropertyWithMultipleOptionsModelDefinition<TOptionCollection, TOption> : ConfigurationPropertyWithMultipleOptionsModelDefinition where TOptionCollection : ICollection<TOption>
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

        public override void SetPropertyValue(IServiceProvider serviceProvider, object config, IEnumerable<string> options)
        {
            var optionLookup = GetAvailableOptions(serviceProvider).ToDictionary(k => k);
            var emptyCollection = CreateNew();
            foreach (var key in options)
            {
                object option;
                if (TryGetOption(serviceProvider, key, out option))
                    emptyCollection.Add((TOption)option);
            }
            SetPropertyValue(config, emptyCollection);
        }


        protected ICollection<TOption> CreateNew()
        {
            if (typeof(TOptionCollection) == typeof(ICollection<TOption>))
                return new List<TOption>();
            var constructor = typeof(TOptionCollection).GetConstructor(Type.EmptyTypes);
            return (TOptionCollection)constructor.Invoke(new object[0]);
        }
    }

    internal abstract class ConfigurationPropertyWithMultipleOptionsModelDefinition : ConfigurationPropertyWithOptionsModelDefinition
    {
        protected ConfigurationPropertyWithMultipleOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType) : base(propertyName, propertyType, propertyParentType, true)
        {

        }

        public abstract void SetPropertyValue(IServiceProvider serviceProvider, object config, IEnumerable<string> options);

    }
}
