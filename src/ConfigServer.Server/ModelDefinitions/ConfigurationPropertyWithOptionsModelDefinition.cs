using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigServer.Server
{
    /// <summary>
    /// Represents the model of the configuration property that contains the information required to build, configure and validate the configuration property.
    /// Implementation for Properties that are selected from a list of options
    /// </summary>
    public abstract class ConfigurationPropertyWithOptionsModelDefinition : ConfigurationPropertyModelBase
    {
        /// <summary>
        /// Initialize ConfigurationPropertyModel with property name
        /// </summary>
        /// <param name="propertyName">configuration property name</param>
        /// <param name="propertyType">configuration property type</param>
        /// <param name="propertyParentType">configuration property parent type</param>
        /// <param name="isMultiSelector">Is property a ICollection that allows for multiple selections</param>
        protected ConfigurationPropertyWithOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType, bool isMultiSelector) : base(propertyName, propertyType, propertyParentType)
        {
            IsMultiSelector = isMultiSelector;
        }

        /// <summary>
        /// Gets all Option definitions for property
        /// </summary>
        /// <param name="serviceProvider">Service provider used to resolve option provider</param>
        /// <returns>Option definitions for property</returns>
        public abstract IEnumerable<ConfigurationPropertyOptionDefintion> GetAvailableOptions(IServiceProvider serviceProvider);

        /// <summary>
        /// Gets Option for property from key
        /// </summary>
        /// <param name="serviceProvider">Service provider used to resolve option provider</param>
        /// <param name="key">key for option</param>
        /// <param name="option">Option from key</param>
        /// <returns>True if option found, false if not</returns>
        public abstract bool TryGetOption(IServiceProvider serviceProvider, string key, out object option);

        /// <summary>
        /// Determines if Option matches key
        /// </summary>
        /// <param name="key">Option Key</param>
        /// <param name="option">Option to be tested</param>
        /// <returns>Does Option matches key</returns>
        public abstract bool OptionMatchesKey(string key, object option);

        /// <summary>
        /// Determines if multiple options can be selected
        /// </summary>
        public bool IsMultiSelector { get; }

    }

    internal class ConfigurationPropertyWithOptionsModelDefinition<TOption,TOptionProvider> : ConfigurationPropertyWithOptionsModelDefinition where TOptionProvider : class
    {
        readonly Func<TOptionProvider, IEnumerable<TOption>> optionProvider;
        readonly Func<TOption, string> keySelector;
        readonly Func<TOption, string> displaySelector;

        internal ConfigurationPropertyWithOptionsModelDefinition(Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector, string propertyName, Type propertyParentType) : base(propertyName,typeof(TOption), propertyParentType, false)
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
            if (option is TOption)
                return key == keySelector((TOption)option);
            return false; 
        }
    }

    internal class ConfigurationPropertyWithOptionsModelDefinition<TOption> : ConfigurationPropertyWithOptionsModelDefinition
    {
        readonly Func<IEnumerable<TOption>> optionProvider;
        readonly Func<TOption, string> keySelector;
        readonly Func<TOption, string> displaySelector;

        internal ConfigurationPropertyWithOptionsModelDefinition(Func<IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector, string propertyName, Type propertyParentType) : base(propertyName, typeof(TOption), propertyParentType, false)
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
            if (option is TOption)
                return key == keySelector((TOption)option);
            return false;
        }
    }

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

    internal abstract class ConfigurationPropertyWithMultipleOptionsModelDefinition  : ConfigurationPropertyWithOptionsModelDefinition
    {
        public ConfigurationPropertyWithMultipleOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType) : base(propertyName, propertyType, propertyParentType, true)
        {

        }

        public abstract void SetPropertyValue(IServiceProvider serviceProvider, object config, IEnumerable<string> options);      
        
    }
}
