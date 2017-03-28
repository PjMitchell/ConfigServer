using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class ConfigurationClientGroupUpdatedEventHandler : IEventHandler<ConfigurationClientGroupUpdatedEvent>
    {
        private readonly IConfigurationClientService clientService;
        public ConfigurationClientGroupUpdatedEventHandler(IConfigurationClientService clientService)
        {
            this.clientService = clientService;
        }

        public Task Handle(ConfigurationClientGroupUpdatedEvent arg)
        {
            clientService.HandleClientGroupUpdated(arg.GroupId);
            return Task.FromResult(true);
        }
    }
}
