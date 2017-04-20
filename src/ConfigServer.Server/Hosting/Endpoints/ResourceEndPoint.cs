using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;

namespace ConfigServer.Server
{
    internal class ResourceEndpoint : IEndpoint
    {
        private readonly IConfigurationClientService configClientService;
        private readonly IResourceStore resourceStore;
        private readonly IHttpResponseFactory httpResponseFactory;
        private const string clientGroupImagePath = "ClientGroupImages";
        public ResourceEndpoint(IConfigurationClientService configClientService, IResourceStore resourceStore, IHttpResponseFactory httpResponseFactory)
        {
            this.configClientService = configClientService;
            this.resourceStore = resourceStore;
            this.httpResponseFactory = httpResponseFactory;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.Request.Method == "GET"
                ?context.CheckAuthorization(options.ServerAuthenticationOptions)
                :context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            // /{id}
            // /{id}/{resource}
            var pathParams = context.ToPathParams();
            if (pathParams.Length == 0 || pathParams.Length > 2)
                return false;

            var clientIdentity = await GetIdentityFromPathOrDefault(pathParams[0]);
            if (clientIdentity == null)
            {
                httpResponseFactory.BuildStatusResponse(context, StatusCodes.Status404NotFound);
                return true;
            }
            if (pathParams.Length == 1)
            {
                var clientResourceCatalogue = await resourceStore.GetResourceCatalogue(clientIdentity);
                await httpResponseFactory.BuildJsonResponse(context, clientResourceCatalogue);
                return true;
            }

            switch (context.Request.Method)
            {
                case "GET":
                {
                    var result = await resourceStore.GetResource(pathParams[1], clientIdentity);
                    if (!result.HasEntry)
                        httpResponseFactory.BuildNotFoundStatusResponse(context);
                    else
                        await httpResponseFactory.BuildFileResponse(context, result.Content, result.Name);
                    break;
                }
                case "POST":
                {
                    var file = context.Request.Form.Files.Single();
                    var uploadRequest = new UpdateResourceRequest
                    {
                        Name = pathParams[1],
                        Identity = clientIdentity,
                        Content = file.OpenReadStream()
                    };
                    await resourceStore.UpdateResource(uploadRequest);
                    httpResponseFactory.BuildNoContentResponse(context);
                    break;
                }
                case "DELETE":
                {
                    await resourceStore.DeleteResources(pathParams[1], clientIdentity);
                    httpResponseFactory.BuildNoContentResponse(context);
                    break;
                }
                default:
                {
                    httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                    break;
                }
                    
            }
            return true;
        }

        private async Task<ConfigurationIdentity> GetIdentityFromPathOrDefault(string pathParam)
        {
            if (string.Equals(pathParam, clientGroupImagePath, StringComparison.OrdinalIgnoreCase))
                return new ConfigurationIdentity(new ConfigurationClient(clientGroupImagePath));
            var client = await configClientService.GetClientOrDefault(pathParam);            
            var clientIdentity = new ConfigurationIdentity(client);
            return clientIdentity;
        }
    }
}
