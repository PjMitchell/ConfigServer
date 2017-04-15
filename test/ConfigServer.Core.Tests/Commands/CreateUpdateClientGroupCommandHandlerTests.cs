using ConfigServer.Server;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class CreateUpdateClientGroupCommandHandlerTests
    {
        private readonly Mock<IConfigClientRepository> configurationClientRepository;
        private readonly Mock<IEventService> eventService;
        private const string groupId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";

        private readonly ICommandHandler<CreateUpdateClientGroupCommand> target;

        public CreateUpdateClientGroupCommandHandlerTests()
        {
            configurationClientRepository = new Mock<IConfigClientRepository>();
            eventService = new Mock<IEventService>();
            target = new CreateUpdateClientGroupCommandHandler(configurationClientRepository.Object, eventService.Object);
        }

        [Fact]
        public async Task Handle_CallsRepository()
        {
            var command = new CreateUpdateClientGroupCommand(new ConfigurationClientGroup { GroupId = groupId });
            await target.Handle(command);
            configurationClientRepository.Verify(r => r.UpdateClientGroupAsync(command.ClientGroup));
        }

        [Fact]
        public async Task Handle_CallsRepository_WithPopulatedIdIfEmpty()
        {
            var command = new CreateUpdateClientGroupCommand(new ConfigurationClientGroup { GroupId = string.Empty });
            await target.Handle(command);
            configurationClientRepository.Verify(r => r.UpdateClientGroupAsync(It.Is<ConfigurationClientGroup>(g=> !string.IsNullOrWhiteSpace(g.GroupId))));
        }

        [Fact]
        public async Task Handle_CallsEventService()
        {
            var command = new CreateUpdateClientGroupCommand(new ConfigurationClientGroup { GroupId = groupId });
            await target.Handle(command);
            eventService.Verify(r => r.Publish(It.Is<ConfigurationClientGroupUpdatedEvent>(e=> e.GroupId == groupId)));
        }

        [Fact]
        public async Task Handle_ReturnsSuccess()
        {
            var command = new CreateUpdateClientGroupCommand(new ConfigurationClientGroup { GroupId = groupId });
            var result = await target.Handle(command);
            Assert.NotNull(result);
            Assert.True(result.IsSuccessful);
        }
    }
}
