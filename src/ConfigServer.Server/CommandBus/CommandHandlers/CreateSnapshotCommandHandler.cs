using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    class CreateSnapshotCommandHandler : ICommandHandler<CreateSnapshotCommand>
    {
        private readonly IConfigurationClientService clientService;
        private readonly IConfigurationService configurationService;
        private readonly IConfigurationSnapshotRepository snapshotRepository;
        private readonly IConfigurationModelRegistry configurationModelRegistry;



        public CreateSnapshotCommandHandler(IConfigurationClientService clientService, IConfigurationService configurationService, IConfigurationSnapshotRepository snapshotRepository, IConfigurationModelRegistry configurationModelRegistry)
        {
            this.clientService = clientService;
            this.configurationService = configurationService;
            this.snapshotRepository = snapshotRepository;
            this.configurationModelRegistry = configurationModelRegistry;
        }

        public async Task<CommandResult> Handle(CreateSnapshotCommand command)
        {
            var client = await clientService.GetClientOrDefault(command.ClientId);
            if (client == null || string.IsNullOrWhiteSpace(client.Group))
                return CommandResult.Failure("Could not find client with group. Can only create snapshot for client with group");
            var models = configurationModelRegistry.GetConfigurationRegistrations(true);
            var version = configurationModelRegistry.GetVersion();
            var configurationIdentity = new ConfigurationIdentity(client, version);
            var snapshotInfo = new SnapshotEntryInfo
            {
                Id = Guid.NewGuid().ToString(),
                GroupId = client.Group,
                Name = command.Name,
                TimeStamp = DateTime.UtcNow
            };
            var configurations = new List<ConfigInstance>();
            foreach (var model in models)
            {
                var configurationInstance = await configurationService.GetAsync(model.ConfigType, configurationIdentity);
                configurations.Add(configurationInstance);
            }

            await snapshotRepository.SaveSnapshot(new ConfigurationSnapshotEntry { Info = snapshotInfo, Configurations = configurations });

            return CommandResult.Success();
        }
    }
}
