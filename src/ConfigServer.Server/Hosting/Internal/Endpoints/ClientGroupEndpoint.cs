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
        private readonly IConfigHttpResponseFactory factory;
        private readonly IConfigClientRepository configurationClientRepository;
        private readonly IEventService eventService;
        private const string noGroupPath = "None";
        private const string groupClientsPath = "Clients";


        public ClientGroupEndpoint(IConfigurationClientService configurationClientService, IConfigClientRepository configurationClientRepository, IConfigHttpResponseFactory factory, IEventService eventService)
        {
            this.configurationClientService = configurationClientService;
            this.configurationClientRepository = configurationClientRepository;
            this.factory = factory;
            this.eventService = eventService;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            // GET Gets All Groups
            // POST Update Groups
            // /{GroupId} GET
            // /{GroupId}/Clients GET
            // /None/Clients GET
            var pathParams = context.ToPathParams();
            switch(pathParams.Length)
            {
                case 0:
                    return await HandleEmptyPath(context);
                case 1:
                    return await HandleGroupPath(context, pathParams[0]);
                case 2:
                    return string.Equals(pathParams[1],groupClientsPath, StringComparison.OrdinalIgnoreCase)
                        ? await HandleGroupClientPath(context, pathParams[0])
                        : await HandleNotFound(context);
                default:
                    return await HandleNotFound(context);
            }
        }

        private async Task<bool> HandleEmptyPath(HttpContext context)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    await factory.BuildResponse(context, await configurationClientService.GetGroups());
                    break;
                case "POST":
                    await HandleGroupSaveRequest(context);
                    break;
                default:
                    factory.BuildMethodNotAcceptedStatusResponse(context);
                    break;
            }
                
            return true;
        }

        private async Task HandleGroupSaveRequest(HttpContext context)
        {
            var group = await context.GetObjectFromJsonBodyAsync<ConfigurationClientGroup>();
            if(group == null)
            {
                await factory.BuildInvalidRequestResponse(context, new[] { "Client group not valid" });
                return;
            }
            if (string.IsNullOrWhiteSpace(group.GroupId))
                group.GroupId = Guid.NewGuid().ToString();
            await configurationClientRepository.UpdateClientGroupAsync(group);
            await eventService.Publish(new ConfigurationClientGroupUpdatedEvent(group.GroupId));
            factory.BuildNoContentResponse(context);
        }

        private async Task<bool> HandleGroupPath(HttpContext context, string groupId)
        {
            var group = await configurationClientService.GetClientGroupOrDefault(groupId);
            if (group != null)
                await factory.BuildResponse(context, group);
            else
                factory.BuildNotFoundStatusResponse(context);
            return true;
        }

        private async Task<bool> HandleGroupClientPath(HttpContext context, string groupId)
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
            await factory.BuildResponse(context, clients);
            return true;
        }

        private Task<bool> HandleNotFound(HttpContext context)
        {
            factory.BuildNotFoundStatusResponse(context);
            return Task.FromResult(true);
        }
    }
}
