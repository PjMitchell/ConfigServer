using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class ConfigEndpoint : IEndpoint
    {
        readonly IConfigInstanceRouter router;
        readonly IConfigurationClientService configurationClientService;
        readonly IHttpResponseFactory httpResponseFactory;

        public ConfigEndpoint(IConfigInstanceRouter router, IConfigurationClientService configurationClientService, IHttpResponseFactory httpResponseFactory)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.router = router;
            this.configurationClientService = configurationClientService;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            if (!CheckMethodAndAuthentication(context, options))
                return;
            var pathParams = context.ToPathParams();
            if (pathParams.Length != 2)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var client = await configurationClientService.GetClientOrDefault(pathParams[0]);
            if (!context.ChallengeClientRead(options, client, httpResponseFactory))
                return;

            var config = await router.GetConfigInstanceOrDefault(client, pathParams[1]);
            if (config == null)
                httpResponseFactory.BuildNotFoundStatusResponse(context);
            else
                await httpResponseFactory.BuildJsonResponse(context, config.GetConfiguration());

        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeAuthentication(options.AllowAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
