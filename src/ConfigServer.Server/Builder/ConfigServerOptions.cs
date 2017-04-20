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
        public ConfigServerAuthenticationOptions ServerAuthenticationOptions { get; set; } = new ConfigServerAuthenticationOptions();

        /// <summary>
        /// Authentication options for Configuration Manager
        /// </summary>
        public ConfigServerAuthenticationOptions ManagerAuthenticationOptions { get; set; } = new ConfigServerAuthenticationOptions();
    }
}
