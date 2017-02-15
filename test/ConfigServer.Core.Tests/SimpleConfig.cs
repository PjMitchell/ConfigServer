using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core.Tests
{
    public class SimpleConfigSet : ConfigurationSet<SimpleConfigSet>
    {
        public Config<SimpleConfig> Config { get; set; }
    }
    public class SimpleConfig
    {
        public int IntProperty { get; set; }
    }
}
