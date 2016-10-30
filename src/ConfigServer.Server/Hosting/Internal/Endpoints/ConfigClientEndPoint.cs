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
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var routePath = context.Request.Path;
            var configSetIds = await configRepository.GetClientsAsync();
            if (string.IsNullOrWhiteSpace(routePath))
            {
                if(!(context.Request.Method == "GET" || context.Request.Method == "POST"))
                    return false;
                if(context.Request.Method == "GET")
                    await responseFactory.BuildResponse(context, configSetIds);
                if (context.Request.Method == "POST")
                    await HandlePost(context);
                return true;
            }
            var queryResult = configSetIds.TryMatchPath(c => c.ClientId, routePath);
            if (!queryResult.HasResult)
                return false;
            await responseFactory.BuildResponse(context, queryResult.QueryResult);

            return true;
        }

        private async Task HandlePost(HttpContext context)
        {
            var data = await context.GetObjectFromJsonBodyAsync<ConfigurationClientPayload>();
            await configRepository.UpdateClientAsync(Map(data));
            responseFactory.BuildNoContentResponse(context);
        }

        private ConfigurationClient Map(ConfigurationClientPayload payload)
        {
            return new ConfigurationClient
            {
                ClientId = string.IsNullOrWhiteSpace(payload.ClientId) ? Guid.NewGuid().ToString() : payload.ClientId,
                Name = payload.Name,
                Description = payload.Description,
                Group = payload.Group,
                Enviroment = payload.Enviroment
            };
        }

        private class ConfigurationClientPayload
        {
            public string ClientId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Group { get; set; }
            public string Enviroment { get; set; }
        }

    }
}
