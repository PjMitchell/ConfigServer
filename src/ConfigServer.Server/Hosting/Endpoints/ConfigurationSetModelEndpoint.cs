using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;
using ConfigServer.Core;

namespace ConfigServer.Server
{
    internal class ConfigurationSetModelEndpoint : IEndpoint
    {
        readonly IHttpResponseFactory httpResponseFactory;
        readonly IConfigurationModelRegistry configCollection;
        readonly IConfigurationSetModelPayloadMapper modelPayloadMapper;
        readonly IConfigurationClientService configClientService;

        public ConfigurationSetModelEndpoint(IHttpResponseFactory httpResponseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigurationModelRegistry configCollection, IConfigurationClientService configClientService)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
            this.configClientService = configClientService;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // Model/{ Client Id}/{ Configuration Set}
            // GET: Model for configuration set
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();

            if (pathParams.Length != 2)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
      
            var client = await configClientService.GetClientOrDefault(pathParams[0]);
            if (!context.ChallengeClientConfigurator(options, client, httpResponseFactory))
                return;

            var configSet = configCollection.SingleOrDefault(c => pathParams[1].Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
            if (configSet == null)
                return;
            await httpResponseFactory.BuildJsonResponse(context, await modelPayloadMapper.Map(configSet, new ConfigurationIdentity(client, configCollection.GetVersion())));
            return;
            
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET" || context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowManagerAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
