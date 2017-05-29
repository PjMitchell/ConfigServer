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

        public Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // GET Gets All
            // POST Update Client
            // /{ClientId} GET
            if (!CheckMethodAndAuthentication(context, options))
                return Task.FromResult(true);

            var pathParams = context.ToPathParams();
            switch (pathParams.Length)
            {
                case 0:
                    return HandleEmptyPath(context);
                case 1:
                    return HandleClientPath(context, pathParams[0]);
                default:
                    return HandleNotFound(context);

            }
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.WriteClaimValue, ConfigServerConstants.ReadClaimValue }, StringComparer.OrdinalIgnoreCase), responseFactory);
            }
            else if (context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.WriteClaimValue }, StringComparer.OrdinalIgnoreCase), responseFactory);
            }
            else
            {
                responseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }

        private Task HandleNotFound(HttpContext context)
        {
            responseFactory.BuildNotFoundStatusResponse(context);
            return Task.FromResult(true);
        }

        private async Task HandleClientPath(HttpContext context, string clientId)
        {
            var client = await configurationClientService.GetClientOrDefault(clientId);
            if (client == null)
                responseFactory.BuildNotFoundStatusResponse(context);
            else
                await responseFactory.BuildJsonResponse(context, Map(client));
        }

        private async Task HandleEmptyPath(HttpContext context)
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
