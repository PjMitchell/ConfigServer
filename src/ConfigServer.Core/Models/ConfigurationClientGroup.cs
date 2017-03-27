using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Core.Models
{
    /// <summary>
    /// Represents a client group
    /// </summary>
    public class ConfigurationClientGroup
    {
        /// <summary>
        /// Id for Group
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// Display name for Group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Image Path
        /// </summary>
        public string ImagePath { get; set; }
    }
}
