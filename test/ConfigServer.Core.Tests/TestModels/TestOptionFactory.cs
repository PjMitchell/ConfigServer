using ConfigServer.Server;
using ConfigServer.Sample.Models;

namespace ConfigServer.Core.Tests.TestModels
{
    public class TestOptionSetFactory : IOptionSetFactory
    {
        public IOptionSet Build(ConfigurationPropertyWithOptionsModelDefinition definition)
        {
            return new OptionSet<Option>(OptionProvider.Options, o => o.Id.ToString(), o => o.Description);
        }
    }
}
