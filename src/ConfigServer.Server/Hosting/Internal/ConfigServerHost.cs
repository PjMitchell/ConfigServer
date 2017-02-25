using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigServer.Server
{
    internal static class ConfigServerHost
    {
        public static Task HandleEndPoint<TEndPoint>(HttpContext context, Func<Task> next, ConfigServerOptions options) where TEndPoint : IEndpoint
        {
            var serviceProvider = context.RequestServices;
            var endPoint = serviceProvider.GetRequiredService<TEndPoint>();
            return HandleEndPoint(endPoint, context, next, options);
        }

        private static async Task HandleEndPoint(IEndpoint endpoint, HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            if (!endpoint.IsAuthorizated(context, options))
            {
                await next.Invoke();
                return;
            }

            var result = await endpoint.TryHandle(context);
            if (!result)
                await next.Invoke();
        }
    }
}
