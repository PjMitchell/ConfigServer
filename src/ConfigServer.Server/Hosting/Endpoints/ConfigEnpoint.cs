using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class ConfigEnpoint : IEndpoint
    {
        readonly IConfigInstanceRouter router;
        readonly IConfigurationClientService configurationClientService;
        readonly IHttpResponseFactory responseFactory;

        public ConfigEnpoint(IConfigInstanceRouter router, IConfigurationClientService configurationClientService, IHttpResponseFactory responseFactory)
        {
            this.responseFactory = responseFactory;
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
                responseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var client = await configurationClientService.GetClientOrDefault(pathParams[0]);
            if (!context.ChallengeClientRead(options, client, responseFactory))
                return;

            var config = await router.GetConfigInstanceOrDefault(client, pathParams[1]);
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
