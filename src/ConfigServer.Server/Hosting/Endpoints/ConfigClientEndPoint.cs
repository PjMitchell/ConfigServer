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

        readonly IHttpResponseFactory httpResponseFactory;
        readonly IConfigurationClientService configurationClientService;
        readonly ICommandBus commandBus;

        public ConfigClientEndPoint(IConfigurationClientService configurationClientService, IHttpResponseFactory httpResponseFactory, ICommandBus commandBus)
        {
            this.httpResponseFactory = httpResponseFactory;
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
                    return HandleEmptyPath(context,options);
                case 1:
                    return HandleClientPath(context, pathParams[0],options);
                default:
                    return HandleNotFound(context);

            }
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowManagerAnomynousAccess, httpResponseFactory);
            }
            else if (context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowManagerAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }

        private Task HandleNotFound(HttpContext context)
        {
            httpResponseFactory.BuildNotFoundStatusResponse(context);
            return Task.FromResult(true);
        }

        private async Task HandleClientPath(HttpContext context, string clientId, ConfigServerOptions options)
        {
            var client = await configurationClientService.GetClientOrDefault(clientId);
            if (context.ChallengeClientConfiguratorOrAdmin(options,client,httpResponseFactory))
                await httpResponseFactory.BuildJsonResponse(context, Map(client));
        }

        private async Task HandleEmptyPath(HttpContext context, ConfigServerOptions options)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await httpResponseFactory.BuildJsonResponse(context,await GetAllClients(context,options));
                    break;
                case "POST":
                    await HandlePost(context);
                    break;
                default:
                    httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                    break;
            }
        }

        private async Task<IEnumerable<ConfigurationClientPayload>> GetAllClients(HttpContext context, ConfigServerOptions options)
        {
            var clients = await configurationClientService.GetClients();
            return context.FilterClientsForUser(clients, options).Select(Map);
        }

        private async Task HandlePost(HttpContext context)
        {
            var data = await context.GetObjectFromJsonBodyAsync<ConfigurationClientPayload>();
            var commandResult = await commandBus.SubmitAsync(new CreateUpdateClientCommand(data));
            await httpResponseFactory.BuildResponseFromCommandResult(context, commandResult);
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
                ReadClaim = payload.ReadClaim,
                ConfiguratorClaim = payload.ConfiguratorClaim,
                Settings = new List<ConfigurationClientSetting>(payload.Settings.Values),
                Tags = payload.Tags
            };
            return result;

        }
    }
}
