using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Core
{
    /// <summary>
    /// Setting for Configuration client
    /// </summary>
    public class ConfigurationClientSetting
    {
        /// <summary>
        /// Key for setting. Case insensitive
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Value of setting
        /// </summary>
        public string Value { get; set; }
    }
}
