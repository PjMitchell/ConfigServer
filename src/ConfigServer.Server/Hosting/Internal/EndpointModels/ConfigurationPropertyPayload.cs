using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationPropertyPayload
    {

        public string PropertyName { get; set; }
        public string PropertyType { get; set; }

        /// <summary>
        /// Display name for property
        /// </summary>
        public string PropertyDisplayName { get; set; }

        /// <summary>
        /// Description for property
        /// </summary>
        public string PropertyDescription { get; set; }

        public Dictionary<string, ConfigurationPropertyPayload> ChildProperty { get; set; }

        public ConfigurationPropertyValidationDefinition ValidationDefinition { get; set; }

        public Dictionary<string, string> Options { get; set; }
    }
}
