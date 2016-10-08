﻿using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationSetModelPayload
    {
        /// <summary>
        /// Configuration set type
        /// </summary>
        public string ConfigurationSetId { get; set; }
        /// <summary>
        /// Display name for ConfigInstance set
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ConfigInstance set description
        /// </summary>
        public string Description { get; set; }

        public Dictionary<string, ConfigurationModelPayload> Config { get; set; }
    }
}
