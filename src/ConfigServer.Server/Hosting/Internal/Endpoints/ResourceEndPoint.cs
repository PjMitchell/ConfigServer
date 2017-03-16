using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Linq;

namespace ConfigServer.Server
{
    internal class ResourceEndpoint : IEndpoint
    {
        private readonly IConfigRepository configRepository;
        private readonly IResourceStore resourceStore;
        private readonly IConfigHttpResponseFactory httpResponseFactory;
        public ResourceEndpoint(IConfigRepository configRepository, IResourceStore resourceStore, IConfigHttpResponseFactory httpResponseFactory)
        {
            this.configRepository = configRepository;
            this.resourceStore = resourceStore;
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
            var pathParams = context.Request.Path.HasValue
                ? context.Request.Path.Value.Split('/')
                : new string[0];
            if (pathParams.Length == 0 || pathParams.Length > 2)
                return false;
            var clients = await configRepository.GetClientsAsync();
            var client = clients.SingleOrDefault(s => s.Name.Equals(pathParams[0], StringComparison.OrdinalIgnoreCase));
            if(client == null)
            {
                httpResponseFactory.BuildStatusResponse(context, StatusCodes.Status404NotFound);
                return true;
            }
            if (pathParams.Length == 1)
            {
                var clientResourceCatalogue = await resourceStore.GetResourceCatalogue(new ConfigurationIdentity(client.ClientId));
                await httpResponseFactory.BuildResponse(context, clientResourceCatalogue);
                return true;
            }


            return true;
        }

        
    }
}
