using Microsoft.AspNetCore.Builder;

namespace ConfigServer.Core.Hosting
{
    public static class ConfigServerBuilderExtension
    {
        public static IApplicationBuilder UseConfigServer(this IApplicationBuilder app)
        {
            app.Use((context, next) => ConfigServerHost.Setup(context, next));
            return app;
        }
    }
}
