using ConfigServer.Sample.Models;
using ConfigServer.Server;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System;

namespace ConfigServer.Core.Tests.Options
{
    public class ConfigurationServiceTests
    {
        private IConfigurationService target;
        private Mock<IConfigurationSetService> configurationSetService;
        private const string clientId = "7aa7d5f0-90fb-420b-a906-d482428a0c44";
        private ConfigurationIdentity identity;

        public ConfigurationServiceTests()
        {
            var registry = new ConfigurationModelRegistry();
            registry.AddConfigurationSet(new SampleConfigSet().BuildConfigurationSetModel());
            configurationSetService = new Mock<IConfigurationSetService>();
            identity = new ConfigurationIdentity(new ConfigurationClient(clientId), new Version(1, 0));
            target = new ConfigurationService(configurationSetService.Object, registry);
        }

        [Fact]
        public async Task GetsConfigFromSets()
        {
            var config = new SampleConfig()
            {
                IsLlamaFarmer = true
            };
            var configSet = new SampleConfigSet()
            {
                SampleConfig = new Config<SampleConfig>(config),
                Instance = identity
            };
            configurationSetService.Setup(set => set.GetConfigurationSet(typeof(SampleConfigSet), identity))
                .ReturnsAsync(configSet);
            var result = await target.GetAsync(typeof(SampleConfig), identity);
            var mappedConfig = (SampleConfig)result.GetConfiguration();

            Assert.Equal(config, mappedConfig);
            Assert.Equal(config.IsLlamaFarmer, mappedConfig.IsLlamaFarmer);

        }
    }
}
