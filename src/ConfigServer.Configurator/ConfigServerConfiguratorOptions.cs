using ConfigServer.Core.Hosting;

namespace ConfigServer.Configurator
{
    public class ConfigServerConfiguratorOptions
    {
        public string Path { get; set; } = "/Configurator";
        public ConfigServerAuthenticationOptions AuthenticationOptions { get; set; } = new ConfigServerAuthenticationOptions();
    }
}
