using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ConfigServer.Core.Hosting
{
    public class ConfigServerHost
    {
        public static async Task Setup(HttpContext context, Func<Task> next, ConfigServerOptions options)
        {
            if(!context.CheckAuthorization(options.AuthenticationOptions))
            {
                await next.Invoke();
                return;
            }

            var serviceProvider = context.RequestServices;
            var router = new ConfigRouter((IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)), (IConfigHttpResponseFactory)serviceProvider.GetService(typeof(IConfigHttpResponseFactory)), (ConfigurationSetCollection)serviceProvider.GetService(typeof(ConfigurationSetCollection)));
            var result = await router.TryHandle(context);
            if (!result)
                await next.Invoke();
        }
    }
}
