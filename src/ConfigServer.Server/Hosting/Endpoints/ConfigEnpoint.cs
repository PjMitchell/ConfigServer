using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ConfigEnpoint : IEndpoint
    {
        readonly IConfigInstanceRouter router;
        readonly IHttpResponseFactory responseFactory;

        public ConfigEnpoint(IConfigInstanceRouter router, IHttpResponseFactory responseFactory)
        {
            this.responseFactory = responseFactory;
            this.router = router;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var config = await router.GetConfigInstanceOrDefault(context.Request.Path);
            if (config == null)
                responseFactory.BuildNotFoundStatusResponse(context);
            else
                await responseFactory.BuildJsonResponse(context, config.GetConfiguration());

        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeAuthentication(options.AllowAnomynousAccess, responseFactory);
            }
            else
            {
                responseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
