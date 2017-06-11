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
                    await GetResourceCatalogue(context, clientIdentity, options);
                    break;
                case 2:
                    await HandleTwoParams(context, pathParams, clientIdentity, options);
                    break;
                case 3:
                    await TransferResource(context, pathParams, clientIdentity, options);
                    break;
                default:
                    httpResponseFactory.BuildNotFoundStatusResponse(context);
                    break;
            }
        }

 

        private async Task GetResourceCatalogue(HttpContext context, ConfigurationIdentity clientIdentity, ConfigServerOptions options)
        {
            if (context.Request.Method != "GET")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }
            if (!context.ChallengeClientConfigurator(options, clientIdentity.Client, httpResponseFactory))
                return;
            var clientResourceCatalogue = await resourceStore.GetResourceCatalogue(clientIdentity);
            await httpResponseFactory.BuildJsonResponse(context, clientResourceCatalogue);
            
        }

        private Task HandleTwoParams(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity, ConfigServerOptions options)
        {
            switch (context.Request.Method)
            {
                case "GET":
                    return GetClientResource(context, pathParams[1], clientIdentity, options);
                case "POST":
                    return SaveClientResource(context, pathParams[1], clientIdentity, options);
                case "DELETE":
                    return DeleteClientResource(context, pathParams[1], clientIdentity, options);
                default:
                    {
                        httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                        return Task.FromResult(true);
                    }
            }
        }

        private async Task GetClientResource(HttpContext context, string resourceName, ConfigurationIdentity clientIdentity, ConfigServerOptions options)
        {
            if (!context.ChallengeClientReadOrConfigurator(options, clientIdentity.Client, httpResponseFactory))
                return;
            var result = await resourceStore.GetResource(resourceName, clientIdentity);
            if (!result.HasEntry)
                httpResponseFactory.BuildNotFoundStatusResponse(context);
            else
                await httpResponseFactory.BuildFileResponse(context, result.Content, result.Name);

        }

        private async Task SaveClientResource(HttpContext context, string resourceName, ConfigurationIdentity clientIdentity, ConfigServerOptions options)
        {
            if (!context.ChallengeClientConfigurator(options, clientIdentity.Client, httpResponseFactory))
                return;

            var file = context.Request.Form.Files.Single();
            var uploadRequest = new UpdateResourceRequest
            {
                Name = resourceName,
                Identity = clientIdentity,
                Content = file.OpenReadStream()
            };
            await resourceStore.UpdateResource(uploadRequest);
            httpResponseFactory.BuildNoContentResponse(context);

        }

        private async Task DeleteClientResource(HttpContext context, string resourceName, ConfigurationIdentity clientIdentity, ConfigServerOptions options)
        {
            if (!context.ChallengeClientConfigurator(options, clientIdentity.Client, httpResponseFactory))
                return;

            await resourceStore.DeleteResources(resourceName, clientIdentity);
            httpResponseFactory.BuildNoContentResponse(context);
        }


        private async Task TransferResource(HttpContext context, string[] pathParams, ConfigurationIdentity clientIdentity, ConfigServerOptions options)
        {
            if(context.Request.Method != "POST")
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return;
            }

            if (!context.ChallengeClientConfigurator(options, clientIdentity.Client, httpResponseFactory))
                return;

            var targetConfigIdentity = await GetIdentityFromPathOrDefault(pathParams[2]);
            if (targetConfigIdentity == null || !pathParams[1].Equals("to", StringComparison.OrdinalIgnoreCase))
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            if (!context.ChallengeClientConfigurator(options, targetConfigIdentity.Client, httpResponseFactory))
                return;
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
