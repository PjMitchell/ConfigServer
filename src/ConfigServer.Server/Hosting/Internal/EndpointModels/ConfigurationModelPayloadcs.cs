using System.Collections.Generic;
namespace ConfigServer.Server
{ 
    internal class ConfigurationModelPayload
    {
        /// <summary>
        /// Display name for ConfigInstance set
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ConfigInstance set description
        /// </summary>
        public string Description { get; set; }

        public Dictionary<string, ConfigurationPropertyPayload> Property { get; set; }
    }
}
