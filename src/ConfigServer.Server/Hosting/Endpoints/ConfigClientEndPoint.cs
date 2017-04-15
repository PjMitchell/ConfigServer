using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigServer.Server
{
    internal class ConfigClientEndPoint : IEndpoint
    {
        readonly IConfigClientRepository configClientRepository;
        readonly IHttpResponseFactory responseFactory;
        readonly IConfigurationClientService configurationClientService;
        readonly IEventService eventService;

        public ConfigClientEndPoint(IConfigurationClientService configurationClientService, IConfigClientRepository configClientRepository, IHttpResponseFactory responseFactory, IEventService eventService)
        {
            this.responseFactory = responseFactory;
            this.configClientRepository = configClientRepository;
            this.configurationClientService = configurationClientService;
            this.eventService = eventService;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            // GET Gets All
            // POST Update Client
            // /{ClientId} GET
            
            var routePath = context.Request.Path;
            var configClients = await configurationClientService.GetClients();
            if (string.IsNullOrWhiteSpace(routePath))
            {
                if(!(context.Request.Method == "GET" || context.Request.Method == "POST"))
                    return false;
                if(context.Request.Method == "GET")
                    await responseFactory.BuildJsonResponse(context, configClients.Select(Map));
                if (context.Request.Method == "POST")
                    await HandlePost(context);
                return true;
            }
            var queryResult = configClients.TryMatchPath(c => c.ClientId, routePath);
            if (!queryResult.HasResult)
                return false;
            await responseFactory.BuildJsonResponse(context,Map(queryResult.QueryResult));

            return true;
        }

        private async Task HandlePost(HttpContext context)
        {
            var data = await context.GetObjectFromJsonBodyAsync<ConfigurationClientPayload>();
            var client = Map(data);
            await configClientRepository.UpdateClientAsync(client);
            await eventService.Publish(new ConfigurationClientUpdatedEvent(client.ClientId));
            responseFactory.BuildNoContentResponse(context);
        }

        private ConfigurationClient Map(ConfigurationClientPayload payload)
        {
            var result = new ConfigurationClient
            {
                ClientId = string.IsNullOrWhiteSpace(payload.ClientId) ? Guid.NewGuid().ToString() : payload.ClientId,
                Name = payload.Name,
                Description = payload.Description,
                Group = payload.Group,
                Enviroment = payload.Enviroment
            };
            foreach (var setting in payload.Settings)
                result.Settings.Add(setting.Key, setting);
            return result;

        }

        private ConfigurationClientPayload Map(ConfigurationClient payload)
        {
            var result = new ConfigurationClientPayload
            {
                ClientId = payload.ClientId,
                Name = payload.Name,
                Description = payload.Description,
                Group = payload.Group,
                Enviroment = payload.Enviroment,
                Settings = new List<ConfigurationClientSetting>(payload.Settings.Values)
            };
            return result;

        }
    }
}
