using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationSetTests
    {
        [Fact]
        public void BuildsDefinitionWithAllConfigs()
        {
            var configSet = new TestConfiguationModule();
            var definition = configSet.BuildConfigurationSetModel();

            Assert.NotNull(definition);
            Assert.Equal(1, definition.Configs.Count());

            var model = definition.Configs.Single();

            Assert.Equal(typeof(SimpleConfig), model.Type);
        }


        private class TestConfiguationModule : ConfigurationSet
        {
            public Config<SimpleConfig> SimpleConfig { get; set; }
        }
    }


}
