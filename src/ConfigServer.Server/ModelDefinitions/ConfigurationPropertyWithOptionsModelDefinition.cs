using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

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
        protected ConfigurationPropertyWithOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType) : base(propertyName, propertyType, propertyParentType)
        {
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

    }

    internal class ConfigurationPropertyWithOptionsModelDefinition<TOptionProvider, TOption> : ConfigurationPropertyWithOptionsModelDefinition where TOptionProvider : class
    {
        readonly Func<TOptionProvider, IEnumerable<TOption>> optionProvider;
        readonly Func<TOption, string> keySelector;
        readonly Func<TOption, string> displaySelector;

        internal ConfigurationPropertyWithOptionsModelDefinition(Func<TOptionProvider, IEnumerable<TOption>> optionProvider, Func<TOption, string> keySelector, Func<TOption, string> displaySelector, string propertyName, Type propertyParentType) : base(propertyName,typeof(TOption), propertyParentType)
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
            if (!(option is TOption))
                return false;

            return key == keySelector((TOption)option);
        }
    }


}
