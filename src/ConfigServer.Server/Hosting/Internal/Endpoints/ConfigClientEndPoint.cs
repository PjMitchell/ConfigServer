using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System;

namespace ConfigServer.Server
{
    internal class ConfigClientEndPoint : IEndpoint
    {
        readonly IConfigRepository configRepository;
        readonly IConfigHttpResponseFactory responseFactory;

        public ConfigClientEndPoint(IConfigRepository configRepository, IConfigHttpResponseFactory responseFactory)
        {
            this.responseFactory = responseFactory;
            this.configRepository = configRepository;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.AuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var configSetIds = await configRepository.GetClientsAsync();
            await responseFactory.BuildResponse(context, configSetIds);
            return true;
        }

    }
}
