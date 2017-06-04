using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ResourceEndpoint : IEndpoint
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

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // /{id}
            // /{id}/{resource}
            // /{id}/to/{id}
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();
            if (pathParams.Length == 0 || pathParams.Length > 3)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }

            var clientIdentity = await GetIdentityFromPathOrDefault(pathParams[0]);
            if (clientIdentity == null)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
                

            switch (pathParams.Length)
            {
                case 1:
                    await HandleSingleParam(context, clientIdentity);
                    break;
                case 2:
                    await HandleTwoParams(context, pathParams, clientIdentity);
                    break;
                case 3:
                    await HandleThreeParams(context, pathParams, clientIdentity);
                    break;
                default:
                    httpResponseFactory.BuildNotFoundStatusResponse(context);
                    break;
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

        private async Task HandleTwoParams(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity)
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
        }

        private async Task HandleThreeParams(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity)
        {
            if(context.Request.Method != "POST")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }

            var targetConfigIdentity = await GetIdentityFromPathOrDefault(pathParams[2]);
            if (targetConfigIdentity == null || !pathParams[1].Equals("to", StringComparison.OrdinalIgnoreCase))
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var filesToCopy = await context.GetObjectFromJsonBodyAsync<string[]>();
            if (filesToCopy.Length > 0)
                await resourceStore.CopyResources(filesToCopy, clientIdentity, targetConfigIdentity);
            httpResponseFactory.BuildNoContentResponse(context);
        }

        private async Task<ConfigurationIdentity> GetIdentityFromPathOrDefault(string pathParam)
        {
            if (string.Equals(pathParam, clientGroupImagePath, StringComparison.OrdinalIgnoreCase))
                return new ConfigurationIdentity(new ConfigurationClient(clientGroupImagePath), registry.GetVersion());
            var client = await configClientService.GetClientOrDefault(pathParam);            
            var clientIdentity = new ConfigurationIdentity(client, registry.GetVersion());
            return clientIdentity;
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeAuthentication(options.AllowAnomynousAccess, httpResponseFactory);
            }
            if(context.Request.Method == "POST" || context.Request.Method == "DELETE")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false;
            }
        }

    }
}
