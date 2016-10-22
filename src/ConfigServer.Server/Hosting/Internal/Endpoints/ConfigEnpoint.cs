using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class ConfigEnpoint : IEndpoint
    {
        readonly IConfigInstanceRouter router;
        readonly IConfigHttpResponseFactory responseFactory;

        public ConfigEnpoint(IConfigInstanceRouter router, IConfigHttpResponseFactory responseFactory)
        {
            this.responseFactory = responseFactory;
            this.router = router;
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var config = await router.GetConfigInstanceOrDefault(context.Request.Path);
            if (config == null)
                return false;
            await responseFactory.BuildResponse(context, config.GetConfiguration());
            return true;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ServerAuthenticationOptions);
        }
    }
}
