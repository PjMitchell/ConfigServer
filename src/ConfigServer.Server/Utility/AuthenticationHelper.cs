using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    /// <summary>
    /// Helper used to check Authorization of ConfigServer requests
    /// </summary>
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Checks Authorization of request and applies status code if not authorized
        /// </summary>
        /// <param name="context">request context.
        /// 401 status code will be applied if user is not authenticated and is required
        /// 403 status code will be applied if user is authenticated but role or claim is incorrect</param>
        /// <param name="options">authentication options</param>
        /// <returns>true if requested is authorized or false if not</returns>
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
