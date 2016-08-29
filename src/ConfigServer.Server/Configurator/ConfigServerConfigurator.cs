using System;
using System.Threading.Tasks;
using ConfigServer.Server.Templates;
using ConfigServer.Core;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal static class ConfigServerConfigurator 
    {
        public static async Task Setup(ConfigServerConfiguratorOptions options,HttpContext context, Func<Task> next)
        {
            if (!context.CheckAuthorization(options.AuthenticationOptions))
            {
                await next.Invoke();
                return;
            }

            var serviceProvider = context.RequestServices;
            var router = new ConfiguratorRouter(serviceProvider, (IConfigurationFormBinder)serviceProvider.GetService(typeof(IConfigurationFormBinder)),(IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)),(ConfigurationSetRegistry)serviceProvider.GetService(typeof(ConfigurationSetRegistry)), new PageBuilder(context));
            var result = await router.HandleRequest(context, options.Path);
            if (!result)
                await next.Invoke();
        }
        
    }
}
