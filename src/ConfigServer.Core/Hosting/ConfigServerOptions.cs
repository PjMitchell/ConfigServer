namespace ConfigServer.Core
{
    public class ConfigServerOptions
    {
        public ConfigServerAuthenticationOptions AuthenticationOptions { get; set; } = new ConfigServerAuthenticationOptions();
    }
}
