using ConfigServer.Server;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class CreateUpdateClientCommandHandlerTests
    {
        private readonly Mock<IConfigClientRepository> configurationClientRepository;
        private readonly Mock<IEventService> eventService;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";

        private readonly ICommandHandler<CreateUpdateClientCommand> target;

        public CreateUpdateClientCommandHandlerTests()
        {
            configurationClientRepository = new Mock<IConfigClientRepository>();
            eventService = new Mock<IEventService>();
            target = new CreateUpdateClientCommandHandler(configurationClientRepository.Object, eventService.Object);
        }

        [Fact]
        public async Task Handle_CallsRepository()
        {

            var command = new CreateUpdateClientCommand(new ConfigurationClientPayload {
                ClientId = clientId,
                Name = "Test Client",
                Description = "Description",
                Enviroment = "Dev",
                Group = "3E37AC18-A00F-47A5-B84E-C79E0823F6D3",
                Settings = new List<ConfigurationClientSetting>
                {
                    new ConfigurationClientSetting { Key = "Password", Value = "1234" }
                }

            });
            ConfigurationClient observed = null;
            configurationClientRepository.Setup(r => r.UpdateClientAsync(It.IsAny<ConfigurationClient>()))
                .Callback((ConfigurationClient arg)=> observed = arg)
                .Returns(()=> Task.FromResult(true));
            await target.Handle(command);

            Assert.NotNull(observed);
            AssertClient(command.Client, observed);
        }

        [Fact]
        public async Task Handle_CallsRepository_WithPopulatedIdIfEmpty()
        {
            var command = new CreateUpdateClientCommand(new ConfigurationClientPayload
            {
                ClientId = string.Empty,
            });
            await target.Handle(command);
            configurationClientRepository.Verify(r => r.UpdateClientAsync(It.Is<ConfigurationClient>(g=> !string.IsNullOrWhiteSpace(g.ClientId))));
        }

        [Fact]
        public async Task Handle_CallsEventService()
        {
            var command = new CreateUpdateClientCommand(new ConfigurationClientPayload
            {
                ClientId = clientId,
            });
            await target.Handle(command);
            eventService.Verify(r => r.Publish(It.Is<ConfigurationClientUpdatedEvent>(e=> e.ClientId == clientId)));
        }

        [Fact]
        public async Task Handle_ReturnsSuccess()
        {
            var command = new CreateUpdateClientCommand(new ConfigurationClientPayload
            {
                ClientId = clientId,
            });
            var result = await target.Handle(command);
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }

        [Fact]
        public async Task Handle_ReturnsFailedIfClientNull()
        {
            var command = new CreateUpdateClientCommand(null);
            var result = await target.Handle(command);
            Assert.NotNull(result);
            Assert.False(result.IsSuccessful);
        }

        private void AssertClient(ConfigurationClientPayload payload, ConfigurationClient client)
        {
            Assert.Equal(payload.ClientId, client.ClientId);
            Assert.Equal(payload.Name, client.Name);
            Assert.Equal(payload.Description, client.Description);
            Assert.Equal(payload.Enviroment, client.Enviroment);
            Assert.Equal(payload.Group, client.Group);
            Assert.Equal(payload.Settings.Select(s => s.Key), client.Settings.Values.Select(s => s.Key));
            Assert.Equal(payload.Settings.Select(s => s.Value), client.Settings.Values.Select(s => s.Value));

        }
    }
}
