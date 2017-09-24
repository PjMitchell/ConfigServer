namespace ConfigServer.Core
{
    internal class SingleClientIdProvider : IClientIdProvider
    {
        private readonly string clientId;

        public SingleClientIdProvider(string clientId)
        {
            this.clientId = clientId;
        }

        public string GetCurrentClientId() => clientId;
    }
}
