using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class ConfigEnpoint : IOldEndpoint
    {
        readonly IConfigInstanceRouter router;
        readonly IHttpResponseFactory responseFactory;

        public ConfigEnpoint(IConfigInstanceRouter router, IHttpResponseFactory responseFactory)
        {
            this.responseFactory = responseFactory;
            this.router = router;
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var config = await router.GetConfigInstanceOrDefault(context.Request.Path);
            if (config == null)
                return false;
            await responseFactory.BuildJsonResponse(context, config.GetConfiguration());
            return true;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ServerAuthenticationOptions);
        }
    }
}
