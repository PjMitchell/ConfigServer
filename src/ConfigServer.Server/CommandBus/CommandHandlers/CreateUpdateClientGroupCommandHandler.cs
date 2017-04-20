using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class CreateUpdateClientGroupCommandHandler : ICommandHandler<CreateUpdateClientGroupCommand>
    {

        private readonly IConfigClientRepository configurationClientRepository;
        private readonly IEventService eventService;

        public CreateUpdateClientGroupCommandHandler(IConfigClientRepository configurationClientRepository, IEventService eventService)
        {
            this.configurationClientRepository = configurationClientRepository;
            this.eventService = eventService;
        }

        public async Task<CommandResult> Handle(CreateUpdateClientGroupCommand command)
        {
            if (command.ClientGroup == null)
                return CommandResult.Failure("Invalid client group");
            
            var group = command.ClientGroup;
            if (string.IsNullOrWhiteSpace(group.GroupId))
                group.GroupId = Guid.NewGuid().ToString();
            await configurationClientRepository.UpdateClientGroupAsync(group);
            await eventService.Publish(new ConfigurationClientGroupUpdatedEvent(group.GroupId));
            return CommandResult.Success();
        }
    }
}
