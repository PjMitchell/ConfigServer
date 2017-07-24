using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace ConfigServer.Server
{
    internal static class ConfigServerHostExtensions
    {
        public static IApplicationBuilder MapEndpoint<TEndpoint>(this IApplicationBuilder app,string path, ConfigServerOptions options) where TEndpoint : IEndpoint
        {
            return app.Map(path, client => client.UseEndpoint<TEndpoint>(options));
        }

        public static IApplicationBuilder UseEndpoint<TEndpoint>(this IApplicationBuilder app, ConfigServerOptions options) where TEndpoint : IEndpoint
        {
            return app.Use((context, next) => HandleEndPoint<TEndpoint>(context, next, options));
        }

        private static Task HandleEndPoint<TEndPoint>(HttpContext context, Func<Task> next, ConfigServerOptions options) where TEndPoint : IEndpoint
        {
            var endPoint = context.RequestServices.GetRequiredService<TEndPoint>();
            return endPoint.Handle(context, options);
        }
    }
}
