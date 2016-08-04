using Microsoft.AspNetCore.Builder;

namespace ConfigServer.Server
{
    /// <summary>
    /// Extension methods for ConfigServer Configurator
    /// </summary>
    public static class ConfiguratorBuilderExtension
    {
        /// <summary>
        /// Use ConfigServer Configurator to manage ConfigServer client configurations
        /// </summary>
        /// <param name="app">The Microsoft.AspNetCore.Builder.IApplicationBuilder.</param>
        /// <param name="options">Options for ConfigServer Configurator</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IApplicationBuilder UseConfigServerConfigurator(this IApplicationBuilder app, ConfigServerConfiguratorOptions options = null)
        {
            app.Use((context, next) => ConfigServerConfigurator.Setup(options?? new ConfigServerConfiguratorOptions(), context, next));
            return app;
        }
    }
}
