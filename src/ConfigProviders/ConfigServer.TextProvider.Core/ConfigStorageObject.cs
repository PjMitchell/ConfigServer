using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.TextProvider.Core
{
    internal class ConfigStorageObject
    {
        public string ServerVersion { get; set; }
        public string ClientId { get; set; }
        public object Config { get; set; }
    }
}
