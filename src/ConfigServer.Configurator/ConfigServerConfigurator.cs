using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using ConfigServer.Configurator.Templates;
using ConfigServer.Core;

namespace ConfigServer.Configurator
{

    public class ConfigServerConfigurator 
    {
        private readonly PathString path;

        private readonly IServiceProvider serviceProvider;

        public ConfigServerConfigurator(PathString path, IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.path = path;
        }
        public async Task Setup(HttpContext context, Func<Task> next)
        {
            var router = new ConfiguratorRouter((IConfigRepository)serviceProvider.GetService(typeof(IConfigRepository)),(ConfigurationSetCollection)serviceProvider.GetService(typeof(ConfigurationSetCollection)), new PageBuilder(context));
            var result = await router.HandleRequest(context, path);
            if (!result)
                await next.Invoke();
        }
        
    }
}
