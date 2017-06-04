using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class ResourceArchiveEndpoint : IEndpoint
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

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // /{id} GET
            // /{id}/{resource} DELETE GET
            // /{id}?before={date} DELETE
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();
            if (pathParams.Length == 0 || pathParams.Length > 2)
            {
                httpResponseFactory.BuildStatusResponse(context, StatusCodes.Status404NotFound);
                return;
            }

            var clientIdentity = await GetIdentityFromPathOrDefault(pathParams[0]);
            if (clientIdentity == null)
            {
                httpResponseFactory.BuildStatusResponse(context, StatusCodes.Status404NotFound);
                return;
            }
            if (pathParams.Length == 1)
            {
                await HandleSingleParam(context, clientIdentity);
                return;
            }
            else
            {
                await HandleTwoParams(context, pathParams, clientIdentity);
            }
            
            return;
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

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }
            else if (context.Request.Method == "DELETE")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
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
