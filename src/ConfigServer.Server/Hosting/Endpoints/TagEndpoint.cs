using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class TagEndpoint : IEndpoint
    {
        private readonly IHttpResponseFactory httpResponseFactory;
        private readonly IConfigurationModelRegistry registry;
        public TagEndpoint(IHttpResponseFactory httpResponseFactory, IConfigurationModelRegistry registry)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.registry = registry;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            if (!CheckMethodAndAuthentication(context, options))
                return;
            var pathParams = context.ToPathParams();
            if (pathParams.Length != 0)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }

            var tags = registry.Select(model => model.RequiredClientTag).Where(model => model != null).Distinct().OrderBy(o => o.Value);
            await httpResponseFactory.BuildJsonResponse(context, tags);
            return;
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return true;
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false;
            }
        }
    }
}
