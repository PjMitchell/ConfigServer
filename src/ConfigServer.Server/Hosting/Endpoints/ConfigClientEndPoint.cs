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

        readonly IHttpResponseFactory responseFactory;
        readonly IConfigurationClientService configurationClientService;
        readonly ICommandBus commandBus;

        public ConfigClientEndPoint(IConfigurationClientService configurationClientService, IHttpResponseFactory responseFactory, ICommandBus commandBus)
        {
            this.responseFactory = responseFactory;
            this.configurationClientService = configurationClientService;
            this.commandBus = commandBus;
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
            var pathParams = context.ToPathParams();
            switch (pathParams.Length)
            {
                case 0:
                    return await HandleEmptyPath(context);
                case 1:
                    return await HandleClientPath(context, pathParams[0]);
                default:
                    return await HandleNotFound(context);

            }
        }

        private Task<bool> HandleNotFound(HttpContext context)
        {
            responseFactory.BuildNotFoundStatusResponse(context);
            return Task.FromResult(true);
        }

        private async Task<bool> HandleClientPath(HttpContext context, string clientId)
        {
            var client = await configurationClientService.GetClientOrDefault(clientId);
            if (client == null)
                responseFactory.BuildNotFoundStatusResponse(context);
            else
                await responseFactory.BuildJsonResponse(context, Map(client));
            return true;
        }

        private async Task<bool> HandleEmptyPath(HttpContext context)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await responseFactory.BuildJsonResponse(context,await GetAllClients());
                    break;
                case "POST":
                    await HandlePost(context);
                    break;
                default:
                    responseFactory.BuildMethodNotAcceptedStatusResponse(context);
                    break;
            }
            return true;
        }

        private async Task<IEnumerable<ConfigurationClientPayload>> GetAllClients()
        {
            var clients = await configurationClientService.GetClients();
            return clients.Select(Map);
        }

        private async Task HandlePost(HttpContext context)
        {
            var data = await context.GetObjectFromJsonBodyAsync<ConfigurationClientPayload>();
            var commandResult = await commandBus.SubmitAsync(new CreateUpdateClientCommand(data));
            await responseFactory.BuildResponseFromCommandResult(context, commandResult);
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
