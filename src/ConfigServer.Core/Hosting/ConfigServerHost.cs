using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Core.Hosting
{
    public class ConfigServerHost
    {
        public static async Task Setup(HttpContext context, Func<Task> next)
        {
            var serviceProvider = context.RequestServices;
            var router = new ConfigRouter((IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)), (IConfigHttpResponseFactory)serviceProvider.GetService(typeof(IConfigHttpResponseFactory)), (ConfigurationSetCollection)serviceProvider.GetService(typeof(ConfigurationSetCollection)));
            var result = await router.TryHandle(context);
            if (!result)
                await next.Invoke();
        }
    }
}
