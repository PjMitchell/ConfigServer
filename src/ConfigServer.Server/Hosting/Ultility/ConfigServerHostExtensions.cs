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
        public static IApplicationBuilder UseEndpoint<TEndpoint>(this IApplicationBuilder app, ConfigServerOptions options) where TEndpoint : IEndpoint
        {
            return app.Use((context, next) => HandleEndPoint<TEndpoint>(context, next, options));
        }

        private static Task HandleEndPoint<TEndPoint>(HttpContext context, Func<Task> next, ConfigServerOptions options) where TEndPoint : IEndpoint
        {
            var serviceProvider = context.RequestServices;
            var endPoint = serviceProvider.GetRequiredService<TEndPoint>();
            return HandleEndPoint(endPoint, context, next, options);
        }

        private static async Task HandleEndPoint(IEndpoint endpoint, HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            if (!endpoint.IsAuthorizated(context, options))
                return;
            var result = await endpoint.TryHandle(context);
        }
    }
}
