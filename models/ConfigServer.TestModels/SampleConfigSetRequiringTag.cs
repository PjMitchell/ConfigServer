using ConfigServer.Server;

namespace ConfigServer.TestModels
{
    [RequiredClientTag("Has Restricted ConfigSet")]
    public class SampleConfigSetRequiringTag : ConfigurationSet<SampleConfigSetRequiringTag>
    {
        public SampleConfigSetRequiringTag() :base("Restricted config set" , "Configuration Set that Requires a Tag to see")
        {

        }
        public Config<SampleConfigRequiringTag> Config { get; set; }
    }

    public class SampleConfigRequiringTag
    {
        public int Number { get; set; }
        public string Value { get; set; }
    }
}
