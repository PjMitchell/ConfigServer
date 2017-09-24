namespace ConfigServer.Client
{
    /// <summary>
    /// Options for configuring ConfigServer client
    /// </summary>
    public class ConfigServerClientOptions
    {
        
        /// <summary>
        /// Uri to ConfigServer
        /// </summary>
        public string ConfigServer { get; set; }

        /// <summary>
        /// Authenticator for client. Applies authentication headers to http client request
        /// Default: Does not apply any authentication details to the http client
        /// </summary>
        public IConfigServerClientAuthenticator Authenticator { get; set; } = new DefaultConfigServerClientAuthenticator();

        /// <summary>
        /// Options for Caching client side configs
        /// </summary>
        public ConfigServerCacheOptions CacheOptions { get; set; } = ConfigServerCacheOptions.Default;
    }
}
