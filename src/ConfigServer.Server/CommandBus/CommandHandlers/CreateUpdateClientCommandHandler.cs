using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class CreateUpdateClientCommandHandler : ICommandHandler<CreateUpdateClientCommand>
    {
        private readonly IConfigClientRepository configClientRepository;
        private readonly IEventService eventService;

        public CreateUpdateClientCommandHandler(IConfigClientRepository configClientRepository, IEventService eventService)
        {
            this.configClientRepository = configClientRepository;
            this.eventService = eventService;
        }

        public async Task<CommandResult> Handle(CreateUpdateClientCommand command)
        {
            if (command.Client == null)
                return CommandResult.Failure("Invalid client");
            var client = Map(command.Client);
            await configClientRepository.UpdateClientAsync(client);
            await eventService.Publish(new ConfigurationClientUpdatedEvent(client.ClientId));
            return CommandResult.Success();
        }

        private ConfigurationClient Map(ConfigurationClientPayload payload)
        {
            var result = new ConfigurationClient
            {
                ClientId = string.IsNullOrWhiteSpace(payload.ClientId) ? Guid.NewGuid().ToString() : payload.ClientId,
                Name = payload.Name,
                Description = payload.Description,
                Group = payload.Group,
                ReadClaim = payload.ReadClaim,
                ConfiguratorClaim = payload.ConfiguratorClaim,
                Enviroment = payload.Enviroment,
                Tags = payload.Tags
            };
            foreach (var setting in payload.Settings)
                result.Settings.Add(setting.Key, setting);
            return result;

        }
    }
}
