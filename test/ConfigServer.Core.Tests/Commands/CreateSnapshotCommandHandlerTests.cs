using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class CreateSnapshotCommandHandlerTests
    {
        private const string groupId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private const string clientId = "9E37AC18-A00F-47A5-B84E-C79E0823F6D4";

        private readonly Mock<IConfigurationClientService> clientService;
        private readonly Mock<IConfigurationService> configurationService;
        private readonly Mock<IConfigurationSnapshotRepository> snapshotRepository;
        private readonly Mock<IConfigurationModelRegistry> configurationModelRegistry;

        private readonly ConfigurationClient client;
        private readonly Version version;
        private readonly ConfigurationRegistration[] configRegistrations;

        ICommandHandler<CreateSnapshotCommand> target;

        public CreateSnapshotCommandHandlerTests()
        {
            client = new ConfigurationClient { ClientId = clientId, Group = groupId };
            version = new Version(1, 0);
            clientService = new Mock<IConfigurationClientService>();
            clientService.Setup(c => c.GetClientOrDefault(clientId))
                .ReturnsAsync(client);
            configurationService = new Mock<IConfigurationService>();
            snapshotRepository = new Mock<IConfigurationSnapshotRepository>();

            configurationModelRegistry = new Mock<IConfigurationModelRegistry>();
            configurationModelRegistry.Setup(r => r.GetVersion())
                .Returns(version);
            configRegistrations = new ConfigurationRegistration[] { new ConfigurationRegistration(typeof(SimpleConfig), nameof(SimpleConfig), false) };
            configurationModelRegistry.Setup(r => r.GetConfigurationRegistrations(true))
                .Returns(() => configRegistrations);
            target = new CreateSnapshotCommandHandler(clientService.Object, configurationService.Object,snapshotRepository.Object, configurationModelRegistry.Object);
        }

        [Fact]
        public async Task Handle_CallsSaveWithCorrectInfo()
        {
            var snapshotName = "NameOne";
            ConfigurationSnapshotEntry observedEntry = null; 
            snapshotRepository.Setup(s => s.SaveSnapshot(It.IsAny<ConfigurationSnapshotEntry>()))
                .Returns((ConfigurationSnapshotEntry entry) =>
                {
                    observedEntry = entry;
                    return Task.FromResult(true);
                });
            var result = await target.Handle(new CreateSnapshotCommand { ClientId = clientId, Name = snapshotName });
            Assert.True(result.IsSuccessful);
            Assert.NotNull(observedEntry);
            Assert.Equal(snapshotName, observedEntry.Info.Name);
            Assert.Equal(groupId, observedEntry.Info.GroupId);
            Assert.True(!string.IsNullOrWhiteSpace(observedEntry.Info.Id));
        }

        [Fact]
        public async Task Handle_CallsSaveWithCorrectConfiguration()
        {
            var snapshotName = "NameOne";
            ConfigurationSnapshotEntry observedEntry = null;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, new ConfigurationIdentity(client, version));
            configurationService.Setup(s => s.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(i => i.Client.Equals(client) && i.ServerVersion.Equals(version))))
                .ReturnsAsync(config);
                
            snapshotRepository.Setup(s => s.SaveSnapshot(It.IsAny<ConfigurationSnapshotEntry>()))
                .Returns((ConfigurationSnapshotEntry entry) =>
                {
                    observedEntry = entry;
                    return Task.FromResult(true);
                });
            var result = await target.Handle(new CreateSnapshotCommand { ClientId = clientId, Name = snapshotName });
            Assert.True(result.IsSuccessful);
            Assert.NotNull(observedEntry);
            Assert.Equal(1, observedEntry.Configurations.Count);
            var observedConfig = observedEntry.Configurations.Single().GetConfiguration() as SimpleConfig;
            Assert.NotNull(observedConfig);
            Assert.Equal(23, observedConfig.IntProperty);
        }

        [Fact]
        public async Task Handle_ReturnsFailedIfClientNotInGroup()
        {
            client.Group = string.Empty;
            var result = await target.Handle(new CreateSnapshotCommand { ClientId = clientId, Name = "Name" });
            Assert.False(result.IsSuccessful);
            snapshotRepository.Verify(s => s.SaveSnapshot(It.IsAny<ConfigurationSnapshotEntry>()), Times.Never);
        }
    }
}
