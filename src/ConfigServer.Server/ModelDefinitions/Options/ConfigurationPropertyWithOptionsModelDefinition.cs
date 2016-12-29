using System;
using System.Collections.Generic;

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
        /// Gets Key from option
        /// </summary>
        /// <param name="option">option to retrieve key from</param>
        /// <returns>Key from Object</returns>
        public abstract string GetKeyFromObject(object option);

        /// <summary>
        /// Determines if multiple options can be selected
        /// </summary>
        public bool IsMultiSelector { get; }

        /// <summary>
        /// Builds OptionSet for Definition
        /// </summary>
        /// <param name="serviceProvider">Service provider</param>
        /// <returns>OptionSet for Definition</returns>
        public abstract IOptionSet BuildOptionSet(IServiceProvider serviceProvider);

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

        public override string GetKeyFromObject(object option) => keySelector((TOption)option);
        
        public override IOptionSet BuildOptionSet(IServiceProvider serviceProvider)
        {
            return new OptionSet<TOption>(GetOptions(serviceProvider), keySelector, displaySelector);
        }

        private IEnumerable<TOption> GetOptions(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService(typeof(TOptionProvider)) as TOptionProvider;
            return optionProvider(provider);
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

        public override string GetKeyFromObject(object option) => keySelector((TOption)option);

        public override IOptionSet BuildOptionSet(IServiceProvider serviceProvider)
        {
            return new OptionSet<TOption>(optionProvider(), keySelector, displaySelector);
        }
    }

}
