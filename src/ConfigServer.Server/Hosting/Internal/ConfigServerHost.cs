using ConfigServer.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal static class ConfigServerHost
    {
        public static Task Setup(HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            var serviceProvider = context.RequestServices;
            var endpoint = new ConfigEnpoint((IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)), (IConfigHttpResponseFactory)serviceProvider.GetService(typeof(IConfigHttpResponseFactory)), (ConfigurationSetRegistry)serviceProvider.GetService(typeof(ConfigurationSetRegistry)));
            return HandleEndPoint(endpoint, context, next, options);
        }

        public static Task SetupClientRouter(HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            var serviceProvider = context.RequestServices;
            var endpoint = new ConfigClientEndPoint((IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)), (IConfigHttpResponseFactory)serviceProvider.GetService(typeof(IConfigHttpResponseFactory)));
            return HandleEndPoint(endpoint, context, next, options);
        }

        public static Task SetupManagerRouter(HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            return HandleEndPoint(new ConfigManagerEndpoint(), context, next, options);
        }

        public static Task HandleEndPoint<TEndPoint>(HttpContext context, Func<Task> next, ConfigServerOptions options) where TEndPoint : IEndpoint
        {
            var serviceProvider = context.RequestServices;
            var endPoint = (TEndPoint)serviceProvider.GetService(typeof(TEndPoint));
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
