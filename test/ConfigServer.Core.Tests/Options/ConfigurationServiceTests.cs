using ConfigServer.Core.Tests.TestModels;
using ConfigServer.InMemoryProvider;
using ConfigServer.Sample.Models;
using ConfigServer.Server;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
namespace ConfigServer.Core.Tests.Options
{
    public class ConfigurationServiceTests
    {
        private IConfigurationService target;
        private Mock<IConfigurationSetService> configurationSetService;
        private InMemoryRepository repository;
        private const string clientId = "7aa7d5f0-90fb-420b-a906-d482428a0c44";

        public ConfigurationServiceTests()
        {
            var registry = new ConfigurationSetRegistry();
            registry.AddConfigurationSet(new SampleConfigSet().BuildConfigurationSetModel());
            configurationSetService = new Mock<IConfigurationSetService>();
            target = new ConfigurationService(configurationSetService.Object, registry);
        }

        [Fact]
        public async Task CheckOptionsAreUpdated()
        {
            var config = new SampleConfig();
            config.Option = new Option { Id = 1, Description = "Not the right description" };
            config.MoarOptions = new List<Option>
            {
                new Option{ Id = 3, Description ="fail"},
                OptionProvider.OptionTwo
            };
            await repository.UpdateConfigAsync(new ConfigInstance<SampleConfig>(config, clientId));
            var result = await target.GetAsync(typeof(SampleConfig), new ConfigurationIdentity(clientId));
            var mappedConfig = (SampleConfig)result.GetConfiguration();

            Assert.Equal(config.Option.Description, OptionProvider.OptionOne.Description);
            Assert.Equal(config.MoarOptions[0].Description, OptionProvider.OptionThree.Description);
            Assert.Equal(config.MoarOptions[1], OptionProvider.OptionTwo);
        }
    }
}
