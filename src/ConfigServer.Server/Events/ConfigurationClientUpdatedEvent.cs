namespace ConfigServer.Server
{
    internal class ConfigurationClientUpdatedEvent : IEvent
    {
        public ConfigurationClientUpdatedEvent(string clientId)
        {
            ClientId = clientId;
        }

        public string ClientId { get; }
    }
}
