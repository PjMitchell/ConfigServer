using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;

namespace ConfigServer.Server
{
    internal class ClientGroupEndpoint : IEndpoint
    {
        private readonly IConfigurationClientService configurationClientService;
        private readonly IHttpResponseFactory factory;
        private readonly ICommandBus commandBus;
        private const string noGroupPath = "None";
        private const string groupClientsPath = "Clients";


        public ClientGroupEndpoint(IConfigurationClientService configurationClientService, IHttpResponseFactory factory, ICommandBus commandBus)
        {
            this.configurationClientService = configurationClientService;
            this.factory = factory;
            this.commandBus = commandBus;
        }

        public Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // GET Gets All Groups
            // POST Update Groups
            // /{GroupId} GET
            // /{GroupId}/Clients GET
            // /None/Clients GET
            if (!CheckMethodAndAuthentication(context, options))
                return Task.FromResult(true);
            

            var pathParams = context.ToPathParams();
            switch(pathParams.Length)
            {
                case 0:
                    return HandleEmptyPath(context);
                case 1:
                    return HandleGroupPath(context, pathParams[0]);
                case 2:
                    return string.Equals(pathParams[1],groupClientsPath, StringComparison.OrdinalIgnoreCase)
                        ? HandleGroupClientPath(context, pathParams[0], options)
                        : HandleNotFound(context);
                default:
                    return HandleNotFound(context);
            }
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, factory);
            }
            else if (context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, factory);
            }
            else
            {
                factory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }

        private async Task HandleEmptyPath(HttpContext context)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await factory.BuildJsonResponse(context,await configurationClientService.GetGroups());
                    break;
                case "POST":
                    await HandleGroupSaveRequest(context);
                    break;
                default:
                    factory.BuildMethodNotAcceptedStatusResponse(context);
                    break;
            }
        }

        private async Task HandleGroupSaveRequest(HttpContext context)
        {
            var group = await context.GetObjectFromJsonBodyAsync<ConfigurationClientGroup>();
            var result = await commandBus.SubmitAsync(new CreateUpdateClientGroupCommand(group));
            await factory.BuildResponseFromCommandResult(context,result);
        }

        private async Task HandleGroupPath(HttpContext context, string groupId)
        {
            var group = await configurationClientService.GetClientGroupOrDefault(groupId);
            if (group != null)
                await factory.BuildJsonResponse(context, group);
            else
                factory.BuildNotFoundStatusResponse(context);
        }

        private async Task HandleGroupClientPath(HttpContext context, string groupId, ConfigServerOptions options)
        {
            IEnumerable<ConfigurationClient> clients;
            if (string.Equals(groupId, noGroupPath, StringComparison.OrdinalIgnoreCase))
            {
                var groups = await configurationClientService.GetGroups();
                var groupIds = new HashSet<string>(groups.Select(s => s.GroupId), StringComparer.OrdinalIgnoreCase);
                clients = (await configurationClientService.GetClients()).Where(c => !groupIds.Contains(c.Group));
            }
            else
            {
                clients = (await configurationClientService.GetClients()).Where(c => string.Equals(groupId, c.Group, StringComparison.OrdinalIgnoreCase));
            }
            if(!string.IsNullOrWhiteSpace(options.ClientAdminClaimType) && !context.HasClaim(options.ClientAdminClaimType, ConfigServerConstants.AdminClaimValue))
                clients = clients.Where(c => context.CheckClientWrite(options, c));
            await factory.BuildJsonResponse(context, clients);
        }

        private Task<bool> HandleNotFound(HttpContext context)
        {
            factory.BuildNotFoundStatusResponse(context);
            return Task.FromResult(true);
        }
    }
}
