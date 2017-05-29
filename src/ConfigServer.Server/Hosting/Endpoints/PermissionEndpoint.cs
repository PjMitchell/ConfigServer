using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class PermissionEndpoint : IEndpoint
    {
        private readonly IHttpResponseFactory factory;

        public PermissionEndpoint(IHttpResponseFactory factory)
        {
            this.factory = factory;
        }

        public Task Handle(HttpContext context, ConfigServerOptions options)
        {
            if(context.Request.Method != "GET")
            {
                factory.BuildMethodNotAcceptedStatusResponse(context);
                return Task.FromResult(true);
            }
            var permissions = GetPermissionFromPrincipal(context.User, options);
            return factory.BuildJsonResponse(context, permissions);            
        }

        private UserPermissions GetPermissionFromPrincipal(ClaimsPrincipal user, ConfigServerOptions options)
        {
            var result = new UserPermissions();
            if(user.HasClaim(s=> s.Type.Equals(options.ClientAdminClaimType, StringComparison.OrdinalIgnoreCase) && ConfigServerConstants.WriteClaimValue.Equals(s.Value, StringComparison.OrdinalIgnoreCase)))
            {
                result.CanAccessClientAdmin = true;
                result.CanEditClients = true;
                result.CanEditGroups = true;
            }
            if(!result.CanAccessClientAdmin && user.HasClaim(s => s.Type.Equals(options.ClientAdminClaimType, StringComparison.OrdinalIgnoreCase) && ConfigServerConstants.ReadClaimValue.Equals(s.Value, StringComparison.OrdinalIgnoreCase)))
            {
                result.CanAccessClientAdmin = true;
            }
            return result;
        }
    }
}
