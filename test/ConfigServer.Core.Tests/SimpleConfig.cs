using ConfigServer.Server;

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
