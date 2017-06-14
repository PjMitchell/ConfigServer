using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Linq;
using ConfigServer.Core;

namespace ConfigServer.Server
{
    internal class PermissionEndpoint : IEndpoint
    {
        private readonly IHttpResponseFactory httpResponseFactory;
        private readonly IConfigurationClientService clientService;

        public PermissionEndpoint(IHttpResponseFactory httpResponseFactory, IConfigurationClientService clientService)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.clientService = clientService;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            if(context.Request.Method != "GET")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }
            
            var permissions = GetPermissionFromPrincipal(context.User, options);
            var pathParams = context.ToPathParams();
            if (pathParams.Length == 0)
                await httpResponseFactory.BuildJsonResponse(context, permissions);
            else
            {
                var client = await clientService.GetClientOrDefault(pathParams[0]);
                var clientPermission = MapToClientPermission(permissions, client);
                await httpResponseFactory.BuildJsonResponse(context, clientPermission);
            }
        }

        private UserClientPermissions MapToClientPermission(UserPermissions permission, ConfigurationClient client)
        {
            return new UserClientPermissions
            {
                CanAccessClientAdmin = true,
                CanEditClients = true,
                CanEditGroups = true,
                CanDeleteArchives = true,
                HasClientConfiguratorClaim = client != null && (string.IsNullOrWhiteSpace(client.ConfiguratorClaim) || permission.ClientConfiguratorClaims.Any(s => s.Equals(client.ConfiguratorClaim, StringComparison.OrdinalIgnoreCase)))
            };
        }

        private UserPermissions GetPermissionFromPrincipal(ClaimsPrincipal user, ConfigServerOptions options)
        {
            var result = new UserPermissions();
            if(user.HasClaim(s=> s.Type.Equals(options.ClientAdminClaimType, StringComparison.OrdinalIgnoreCase) && ConfigServerConstants.AdminClaimValue.Equals(s.Value, StringComparison.OrdinalIgnoreCase)))
            {
                result.CanAccessClientAdmin = true;
                result.CanEditClients = true;
                result.CanEditGroups = true;
                result.CanDeleteArchives = true;
            }
            if(!result.CanAccessClientAdmin && user.HasClaim(s => s.Type.Equals(options.ClientAdminClaimType, StringComparison.OrdinalIgnoreCase) && ConfigServerConstants.ConfiguratorClaimValue.Equals(s.Value, StringComparison.OrdinalIgnoreCase)))
            {
                result.CanAccessClientAdmin = true;
            }
            result.ClientConfiguratorClaims = user.Claims.Where(w => w.Type.Equals(options.ClientConfiguratorClaimType, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Value)
                .ToArray();
            return result;
        }
    }
}
