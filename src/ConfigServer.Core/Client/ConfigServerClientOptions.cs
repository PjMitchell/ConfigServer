using ConfigServer.Core.Client;

namespace ConfigServer.Core
{
    public class ConfigServerClientOptions
    {
        public string ApplicationId { get; set; }
        public string ConfigServer { get; set; }
        public IConfigServerClientAuthenticator Authenticator { get; set; } = new DefaultConfigServerClientAuthenticator();
    }
}
