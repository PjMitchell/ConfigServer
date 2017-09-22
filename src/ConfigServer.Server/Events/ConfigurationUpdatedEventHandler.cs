using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class ConfigurationUpdatedEventHandler : IEventHandler<ConfigurationUpdatedEvent>
    {
        private IConfigurationSetService configurationSetService;

        public ConfigurationUpdatedEventHandler(IConfigurationSetService configurationSetService)
        {
            this.configurationSetService = configurationSetService;
        }

        public Task Handle(ConfigurationUpdatedEvent arg)
        {
            return configurationSetService.HandleConfigurationUpdatedEvent(arg);
        }
    }
}
