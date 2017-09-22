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
            return clientService.HandleClientGroupUpdated(arg.GroupId);
        }
    }
}
