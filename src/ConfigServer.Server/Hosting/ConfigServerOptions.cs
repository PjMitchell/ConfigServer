namespace ConfigServer.Server
{
    /// <summary>
    /// ConfigServer options
    /// </summary>
    public class ConfigServerOptions
    {
        /// <summary>
        /// Authentication options for ConfigServer
        /// </summary>
        public ConfigServerAuthenticationOptions AuthenticationOptions { get; set; } = new ConfigServerAuthenticationOptions();
    }
}
