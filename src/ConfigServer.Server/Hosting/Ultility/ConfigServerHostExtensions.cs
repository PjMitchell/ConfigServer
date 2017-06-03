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
        public static IApplicationBuilder UseOldEndpoint<TEndpoint>(this IApplicationBuilder app, ConfigServerOptions options) where TEndpoint : IOldEndpoint
        {
            return app.Use((context, next) => HandleEndPointOld<TEndpoint>(context, next, options));
        }

        public static IApplicationBuilder UseEndpoint<TEndpoint>(this IApplicationBuilder app, ConfigServerOptions options) where TEndpoint : IEndpoint
        {
            return app.Use((context, next) => HandleEndPoint<TEndpoint>(context, next, options));
        }

        private static Task HandleEndPointOld<TEndPoint>(HttpContext context, Func<Task> next, ConfigServerOptions options) where TEndPoint : IOldEndpoint
        {
            var serviceProvider = context.RequestServices;
            var endPoint = serviceProvider.GetRequiredService<TEndPoint>();
            return HandleEndPoint(endPoint, context, next, options);
        }

        private static Task HandleEndPoint<TEndPoint>(HttpContext context, Func<Task> next, ConfigServerOptions options) where TEndPoint : IEndpoint
        {
            var endPoint = context.RequestServices.GetRequiredService<TEndPoint>();
            return endPoint.Handle(context, options);
        }

        private static async Task HandleEndPoint(IOldEndpoint endpoint, HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            if (!endpoint.IsAuthorizated(context, options))
                return;
            var result = await endpoint.TryHandle(context);
        }
    }
}
