﻿using System.Collections.Generic;
namespace ConfigServer.Server
{ 
    internal class ConfigurationModelPayload
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsOption { get; set; }
        public string KeyPropertyName { get; set; }

        public Dictionary<string, ConfigurationPropertyPayload> Property { get; set; }
    }
}
