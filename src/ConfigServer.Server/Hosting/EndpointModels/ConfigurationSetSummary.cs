using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigurationSetSummary
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

        public string RequiredClientTag { get; set; }

        public ICollection<ConfigurationModelSummary> Configs { get; set; }
    }
}
