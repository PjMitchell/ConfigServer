using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class ConfigurationPropertyBuilder
    {
        private readonly ConfigurationPropertyDefinition definition;

        public ConfigurationPropertyBuilder(ConfigurationPropertyDefinition definition)
        {
            this.definition = definition;
        }

        public ConfigurationPropertyBuilder WithDisplayName(string name)
        {
            definition.PropertyDisplayName = name;
            return this;
        }

        public ConfigurationPropertyBuilder WithDiscription(string description)
        {
            definition.PropertyDescription = description;
            return this;
        }
    }
}
