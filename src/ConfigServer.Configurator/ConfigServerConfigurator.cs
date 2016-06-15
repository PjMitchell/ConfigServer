using System;
using System.Threading.Tasks;
using ConfigServer.Configurator.Templates;
using ConfigServer.Core;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Configurator
{

    public class ConfigServerConfigurator 
    {
        public static async Task Setup(PathString path,HttpContext context, Func<Task> next)
        {
            var serviceProvider = context.RequestServices;
            var router = new ConfiguratorRouter((IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)),(ConfigurationSetCollection)serviceProvider.GetService(typeof(ConfigurationSetCollection)), new PageBuilder(context));
            var result = await router.HandleRequest(context, path);
            if (!result)
                await next.Invoke();
        }
        
    }
}
