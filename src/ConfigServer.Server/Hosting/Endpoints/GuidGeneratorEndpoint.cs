using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class GuidGeneratorEndpoint : IEndpoint
    {
        private readonly IHttpResponseFactory httpResponseFactory;

        public GuidGeneratorEndpoint(IHttpResponseFactory httpResponseFactory)
        {
            this.httpResponseFactory = httpResponseFactory;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options) => true;

        public async Task<bool> TryHandle(HttpContext context)
        {
            var param = context.ToPathParams();
            if (param.Length != 0)
                httpResponseFactory.BuildNotFoundStatusResponse(context);
            else if (context.Request.Method != "POST")
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
            else
                await httpResponseFactory.BuildJsonResponse(context, Guid.NewGuid());
            return true;
        }
    }
}
