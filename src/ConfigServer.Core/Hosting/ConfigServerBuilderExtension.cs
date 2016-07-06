using Microsoft.AspNetCore.Builder;

namespace ConfigServer.Core.Hosting
{
    public static class ConfigServerBuilderExtension
    {
        public static IApplicationBuilder UseConfigServer(this IApplicationBuilder app, ConfigServerOptions options = null)
        {
            options = options ?? new ConfigServerOptions();
            app.Use((context, next) => ConfigServerHost.Setup(context, next, options));
            return app;
        }
    }
}
