namespace ConfigServer.Client
{
    /// <summary>
    /// Options for configuring ConfigServer client
    /// </summary>
    public class ConfigServerClientOptions
    {
        /// <summary>
        /// Identifier for application requesting the configuration
        /// </summary>
        public string ClientId { get; set; }
        
        /// <summary>
        /// Uri to ConfigServer
        /// </summary>
        public string ConfigServer { get; set; }

        /// <summary>
        /// Authenticator for client. Applies authentication headers to http client request
        /// Default: Does not apply any authentication details to the http client
        /// </summary>
        public IConfigServerClientAuthenticator Authenticator { get; set; } = new DefaultConfigServerClientAuthenticator();
    }
}
