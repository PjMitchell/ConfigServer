using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class ConfigurationClientUpdatedEventHandler : IEventHandler<ConfigurationClientUpdatedEvent>
    {
        private readonly IConfigurationClientService clientService;
        public ConfigurationClientUpdatedEventHandler(IConfigurationClientService clientService)
        {
            this.clientService = clientService;
        }

        public Task Handle(ConfigurationClientUpdatedEvent arg)
        {
            return clientService.HandleClientUpdated(arg.ClientId);
        }
    }
}
