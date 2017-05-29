using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;

namespace ConfigServer.Server
{
    internal class ResourceArchiveEndpoint : IOldEndpoint
    {
        private readonly IConfigurationClientService configClientService;
        private readonly IResourceArchive resourceArchive;
        private readonly IHttpResponseFactory httpResponseFactory;
        private readonly IConfigurationSetRegistry registry;
        public ResourceArchiveEndpoint(IConfigurationClientService configClientService,IConfigurationSetRegistry registry, IResourceArchive resourceArchive, IHttpResponseFactory httpResponseFactory)
        {
            this.configClientService = configClientService;
            this.resourceArchive = resourceArchive;
            this.httpResponseFactory = httpResponseFactory;
            this.registry = registry;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            // /{id} GET
            // /{id}/{resource} DELETE GET
            // /{id}?before={date} DELETE
            var pathParams = context.ToPathParams();
            if (pathParams.Length == 0 || pathParams.Length > 2)
            {
                httpResponseFactory.BuildStatusResponse(context, StatusCodes.Status404NotFound);
                return true;
            }

            var clientIdentity = await GetIdentityFromPathOrDefault(pathParams[0]);
            if (clientIdentity == null)
            {
                httpResponseFactory.BuildStatusResponse(context, StatusCodes.Status404NotFound);
                return true;
            }
            if (pathParams.Length == 1)
            {
                await HandleSingleParam(context, clientIdentity);
                return true;
            }
            else
            {
                await HandleTwoParams(context, pathParams, clientIdentity);
            }
            
            return true;
        }

        private async Task HandleSingleParam(HttpContext context, ConfigurationIdentity clientIdentity)
        {
            

            switch (context.Request.Method)
            {
                case "GET":
                    {
                        var clientResourceCatalogue = await resourceArchive.GetArchiveResourceCatalogue(clientIdentity);
                        await httpResponseFactory.BuildJsonResponse(context, clientResourceCatalogue);
                        break;
                    }
                case "DELETE":
                    {
                        if(context.Request.Query.TryGetValue("before", out var pram) && DateTime.TryParse(pram, out var dateParam))
                        {
                            await resourceArchive.DeleteOldArchiveResources(dateParam, clientIdentity);
                            httpResponseFactory.BuildNoContentResponse(context);
                            break;
                        }
                        httpResponseFactory.BuildNotFoundStatusResponse(context);
                        break;                      
                    }
                default:
                    {
                        httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                        break;
                    }

            }
        }

        private async Task HandleTwoParams(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    {
                        var result = await resourceArchive.GetArchiveResource(pathParams[1], clientIdentity);
                        if (!result.HasEntry)
                            httpResponseFactory.BuildNotFoundStatusResponse(context);
                        else
                            await httpResponseFactory.BuildFileResponse(context, result.Content, result.Name);
                        break;
                    }
                case "DELETE":
                    {
                        await resourceArchive.DeleteArchiveResource(pathParams[1], clientIdentity);
                        httpResponseFactory.BuildNoContentResponse(context);
                        break;
                    }
                default:
                    {
                        httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                        break;
                    }

            }
        }

        private async Task<ConfigurationIdentity> GetIdentityFromPathOrDefault(string pathParam)
        {
            var client = await configClientService.GetClientOrDefault(pathParam);            
            var clientIdentity = new ConfigurationIdentity(client, registry.GetVersion());
            return clientIdentity;
        }
    }
}
