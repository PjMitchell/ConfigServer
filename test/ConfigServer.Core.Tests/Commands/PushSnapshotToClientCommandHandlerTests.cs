using ConfigServer.Sample.Models;
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
    public class PushSnapshotToClientCommandHandlerTests
    {
        private readonly Mock<IConfigurationSnapshotRepository> repo;
        private readonly Mock<IConfigurationModelRegistry> reg;
        private readonly Mock<IConfigRepository> configRepository;
        private readonly Mock<IEventService> eventService;
        private readonly ICommandHandler<PushSnapshotToClientCommand> target;
        private readonly Version version;
        private const string snapShotId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private const string clientId = "9E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private ConfigurationClient client;
        private ICollection<ConfigInstance> configs;



        public PushSnapshotToClientCommandHandlerTests()
        {

            reg = new Mock<IConfigurationModelRegistry>();
            client = new ConfigurationClient(clientId);
            version = new Version(1, 0);
            reg.Setup(s => s.GetVersion())
                .Returns(version);
            repo = new Mock<IConfigurationSnapshotRepository>();
            repo.Setup(s => s.GetSnapshot(snapShotId, It.Is<ConfigurationIdentity>(i => i.Client.Equals(client) && i.ServerVersion.Equals(version))))
                .ReturnsAsync(() => new ConfigurationSnapshotEntry { Configurations = configs });
            configs = new List<ConfigInstance>();
            configRepository = new Mock<IConfigRepository>();
            eventService = new Mock<IEventService>();
            target = new PushSnapshotToClientCommandHandler(repo.Object, reg.Object, configRepository.Object, eventService.Object);
        }

        [Fact]
        public async Task PushSavesConfiguration()
        {
            var sampleConfigInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, new ConfigurationIdentity(client, version));
            configs.Add(sampleConfigInstance);
            var command = new PushSnapshotToClientCommand { SnapshotId = snapShotId, TargetClient = client, ConfigsToCopy = new string[] { sampleConfigInstance.Name } };
            var result = await target.Handle(command);
            Assert.True(result.IsSuccessful);
            configRepository.Verify(c => c.UpdateConfigAsync(sampleConfigInstance), Times.Once);
        }

        [Fact]
        public async Task PushSavesConfiguration_RaisesEvent()
        {
            var sampleConfigInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, new ConfigurationIdentity(client, version));
            configs.Add(sampleConfigInstance);
            var command = new PushSnapshotToClientCommand { SnapshotId = snapShotId, TargetClient = client, ConfigsToCopy = new string[] { sampleConfigInstance.Name } };
            var result = await target.Handle(command);
            Assert.True(result.IsSuccessful);
            eventService.Verify(c => c.Publish(It.Is<ConfigurationUpdatedEvent>(e=> e.ConfigurationType == sampleConfigInstance.ConfigType && e.Identity.Client.Equals(client) && e.Identity.ServerVersion.Equals(version))), Times.Once);
        }

        [Fact]
        public async Task PushSavesConfiguration_OnlySpecifiedConfig()
        {
            var sampleConfigInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, new ConfigurationIdentity(client, version));
            var notThisConfigInstance = new ConfigInstance<SampleConfig>(new SampleConfig { LlamaCapacity = 23 }, new ConfigurationIdentity(client, version));

            configs.Add(sampleConfigInstance);
            configs.Add(notThisConfigInstance);

            var command = new PushSnapshotToClientCommand { SnapshotId = snapShotId, TargetClient = client, ConfigsToCopy = new string[] { sampleConfigInstance.Name } };
            var result = await target.Handle(command);
            Assert.True(result.IsSuccessful);
            configRepository.Verify(c => c.UpdateConfigAsync(sampleConfigInstance), Times.Once);
            configRepository.Verify(c => c.UpdateConfigAsync(notThisConfigInstance), Times.Never);

        }
    }
}
