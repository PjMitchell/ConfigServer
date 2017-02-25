using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal abstract class ConfigurationPropertyWithOptionsModelDefinition : ConfigurationPropertyModelBase, IOptionPropertyDefinition
    {
        protected ConfigurationPropertyWithOptionsModelDefinition(string propertyName, Type propertyType, Type propertyParentType, bool isMultiSelector) : base(propertyName, propertyType, propertyParentType)
        {
            IsMultiSelector = isMultiSelector;
        }

        public abstract string GetKeyFromObject(object option);

        public bool IsMultiSelector { get; }

        public abstract IOptionSet BuildOptionSet(IServiceProvider serviceProvider, ConfigurationIdentity configIdentity);

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

        public override IOptionSet BuildOptionSet(IServiceProvider serviceProvider, ConfigurationIdentity configIdentity)
        {
            return new OptionSet<TOption>(optionProvider(), keySelector, displaySelector);
        }
    }
}
