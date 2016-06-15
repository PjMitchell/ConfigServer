using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Configurator
{
    public static class ConfiguratorBuilderExtension
    {
        public static IApplicationBuilder UseConfigServerConfigurator(this IApplicationBuilder app, string path)
        {
            app.Use((context, next) => ConfigServerConfigurator.Setup(path, context, next));
            return app;
        }

        public static IApplicationBuilder UseConfigServerConfigurator(this IApplicationBuilder app)
        {
            return app.UseConfigServerConfigurator("/Configurator");
        }

        public static IServiceCollection UseUseConfigServerConfigurator(this IServiceCollection collection)
        {
            return collection;
        }
    }
}
