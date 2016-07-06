using ConfigServer.Core.Hosting;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Core
{
    public static class AuthenticationHelper
    {
        public static bool CheckAuthorization(this HttpContext context, ConfigServerAuthenticationOptions options)
        {
            if (!options.RequireAuthentication)
                return true;
            if (!context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return false;
            }


            if (!string.IsNullOrWhiteSpace(options.RequiredRole) && !context.User.IsInRole(options.RequiredRole))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return false;
            }
            if (options.RequiredClaim != null && !context.User.HasClaim(options.RequiredClaim))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return false;
            }
            return true;
        }
    }
}
