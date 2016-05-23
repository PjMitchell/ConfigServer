using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public abstract class ConfigurationPropertyBuilder<T> where T :ConfigurationPropertyBuilder<T>
    {
        protected readonly ConfigurationPropertyDefinition definition;

        protected ConfigurationPropertyBuilder(ConfigurationPropertyDefinition definition)
        {
            this.definition = definition;
        }

        public T WithDisplayName(string name)
        {
            definition.PropertyDisplayName = name;
            return (T)this;
        }

        public T WithDiscription(string description)
        {
            definition.PropertyDescription = description;
            return (T)this;
        }
    }

    public class ConfigurationIntergerPropertyBuilder : ConfigurationPropertyBuilder<ConfigurationIntergerPropertyBuilder>
    {
        public ConfigurationIntergerPropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }
        public ConfigurationIntergerPropertyBuilder WithMaxValue(long value)
        {
            definition.ValidationRules.Max = value;
            return this;
        }

        public ConfigurationIntergerPropertyBuilder WithMinValue(long value)
        {
            definition.ValidationRules.Max = value;
            return this;
        }        
    }

    public class ConfigurationFloatPropertyBuilder : ConfigurationPropertyBuilder<ConfigurationFloatPropertyBuilder>
    {
        public ConfigurationFloatPropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }

    }

    public class ConfigurationStringPropertyBuilder : ConfigurationPropertyBuilder<ConfigurationStringPropertyBuilder>
    {
        public ConfigurationStringPropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }

    }

    public class ConfigurationBoolPropertyBuilder : ConfigurationPropertyBuilder<ConfigurationBoolPropertyBuilder>
    {
        public ConfigurationBoolPropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }

    }

    public class ConfigurationEnumPropertyBuilder : ConfigurationPropertyBuilder<ConfigurationEnumPropertyBuilder>
    {
        public ConfigurationEnumPropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }

    }

    public class ConfigurationDateTimePropertyBuilder : ConfigurationPropertyBuilder<ConfigurationDateTimePropertyBuilder>
    {
        public ConfigurationDateTimePropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }

    }

    public class ConfigurationModelPropertyBuilder : ConfigurationPropertyBuilder<ConfigurationModelPropertyBuilder>
    {
        public ConfigurationModelPropertyBuilder(ConfigurationPropertyDefinition definition) : base(definition) { }

    }
}
