using System;
using System.Collections.Generic;
using System.Text;
using ConfigServer.Core;
using System.Threading.Tasks;
using System.Linq;

namespace ConfigServer.Server
{
    internal class PushSnapshotToClientCommandHandler : ICommandHandler<PushSnapshotToClientCommand>
    {
        private readonly IConfigurationSnapshotRepository repo;
        private readonly IConfigurationModelRegistry registry;
        private readonly IConfigRepository configRepository;
        private readonly IEventService eventService;

        public PushSnapshotToClientCommandHandler(IConfigurationSnapshotRepository repo, IConfigurationModelRegistry registry, IConfigRepository configRepository, IEventService eventService)
        {
            this.repo = repo;
            this.registry = registry;
            this.configRepository = configRepository;
            this.eventService = eventService;
        }

        public async Task<CommandResult> Handle(PushSnapshotToClientCommand command)
        {
            var version = registry.GetVersion();
            var snapshotEntry =await repo.GetSnapshot(command.SnapshotId, new ConfigurationIdentity(command.TargetClient, version));
            foreach (var instance in GetConfigurations(snapshotEntry, command))
            {
                await configRepository.UpdateConfigAsync(instance);
                await eventService.Publish(new ConfigurationUpdatedEvent(instance));
            }
            return CommandResult.Success();
        }

        private IEnumerable<ConfigInstance> GetConfigurations(ConfigurationSnapshotEntry entry, PushSnapshotToClientCommand command)
        {
            if (command.ConfigsToCopy == null || command.ConfigsToCopy.Length == 0)
                return entry.Configurations;
            return entry.Configurations.Where(w => command.ConfigsToCopy.Contains(w.Name, StringComparer.OrdinalIgnoreCase));
        }
    }
}
