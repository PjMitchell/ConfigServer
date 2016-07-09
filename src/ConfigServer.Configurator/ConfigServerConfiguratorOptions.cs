using ConfigServer.Core;

namespace ConfigServer.Configurator
{
    /// <summary>
    /// Options for ConfigServer configurator
    /// </summary>
    public class ConfigServerConfiguratorOptions
    {
        /// <summary>
        /// Path to configurator
        /// Default: /Configurator
        /// </summary>
        public string Path { get; set; } = "/Configurator";

        /// <summary>
        /// Authentication options for ConfigServer configurator
        /// </summary>
        public ConfigServerAuthenticationOptions AuthenticationOptions { get; set; } = new ConfigServerAuthenticationOptions();
    }
}
