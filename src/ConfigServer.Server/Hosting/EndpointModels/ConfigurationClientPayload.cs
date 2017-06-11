using ConfigServer.Core;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationClientPayload
    {
        public ConfigurationClientPayload()
        {
            Settings = new List<ConfigurationClientSetting>();
        }
        public string ClientId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Group { get; set; }
        public string Enviroment { get; set; }
        public string ReadClaim { get; set; }
        public string ConfiguratorClaim { get; set; }
        public List<ConfigurationClientSetting> Settings { get; set; }
    }
}
