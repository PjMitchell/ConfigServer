using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;

namespace ConfigServer.Server
{
    internal class ResourceEndpoint : IOldEndpoint
    {
        private readonly IConfigurationClientService configClientService;
        private readonly IResourceStore resourceStore;
        private readonly IHttpResponseFactory httpResponseFactory;
        private readonly IConfigurationSetRegistry registry;
        private const string clientGroupImagePath = "ClientGroupImages";
        public ResourceEndpoint(IConfigurationClientService configClientService,IConfigurationSetRegistry registry, IResourceStore resourceStore, IHttpResponseFactory httpResponseFactory)
        {
            this.configClientService = configClientService;
            this.resourceStore = resourceStore;
            this.httpResponseFactory = httpResponseFactory;
            this.registry = registry;
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
            // /{id}/to/{id}
            var pathParams = context.ToPathParams();
            if (pathParams.Length == 0 || pathParams.Length > 3)
                return HandleNotFound(context);

            var clientIdentity = await GetIdentityFromPathOrDefault(pathParams[0]);
            if (clientIdentity == null)
                return HandleNotFound(context);

            switch (pathParams.Length)
            {
                case 1:
                    return await HandleSingleParam(context, clientIdentity);
                case 2:
                    return await HandleTwoParams(context, pathParams, clientIdentity);
                case 3:
                    return await HandleThreeParams(context, pathParams, clientIdentity);
                default:
                    return HandleNotFound(context);
            }
        }

 

        private async Task<bool> HandleSingleParam(HttpContext context, ConfigurationIdentity clientIdentity)
        {
            if (context.Request.Method != "GET")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return true;
            }
            var clientResourceCatalogue = await resourceStore.GetResourceCatalogue(clientIdentity);
            await httpResponseFactory.BuildJsonResponse(context, clientResourceCatalogue);
            return true;
        }

        private async Task<bool> HandleTwoParams(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity)
        {
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

        private async Task<bool> HandleThreeParams(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity)
        {
            if(context.Request.Method != "POST")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return true;
            }

            var targetConfigIdentity = await GetIdentityFromPathOrDefault(pathParams[2]);
            if (targetConfigIdentity == null || !pathParams[1].Equals("to", StringComparison.OrdinalIgnoreCase))
                return HandleNotFound(context);
            var filesToCopy = await context.GetObjectFromJsonBodyAsync<string[]>();
            if (filesToCopy.Length > 0)
                await resourceStore.CopyResources(filesToCopy, clientIdentity, targetConfigIdentity);
            httpResponseFactory.BuildNoContentResponse(context);
            return true;
        }

        private async Task<ConfigurationIdentity> GetIdentityFromPathOrDefault(string pathParam)
        {
            if (string.Equals(pathParam, clientGroupImagePath, StringComparison.OrdinalIgnoreCase))
                return new ConfigurationIdentity(new ConfigurationClient(clientGroupImagePath), registry.GetVersion());
            var client = await configClientService.GetClientOrDefault(pathParam);            
            var clientIdentity = new ConfigurationIdentity(client, registry.GetVersion());
            return clientIdentity;
        }

        private bool HandleNotFound(HttpContext context)
        {
            httpResponseFactory.BuildNotFoundStatusResponse(context);
            return true;
        }
    }
}
