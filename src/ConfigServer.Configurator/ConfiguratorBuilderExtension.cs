using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Configurator
{
    public static class ConfiguratorBuilderExtension
    {
        public static IApplicationBuilder UseConfigServerConfigurator(this IApplicationBuilder app, ConfigServerConfiguratorOptions options = null)
        {
            app.Use((context, next) => ConfigServerConfigurator.Setup(options?? new ConfigServerConfiguratorOptions(), context, next));
            return app;
        }

        public static IServiceCollection UseUseConfigServerConfigurator(this IServiceCollection collection)
        {
            return collection;
        }
    }
}
