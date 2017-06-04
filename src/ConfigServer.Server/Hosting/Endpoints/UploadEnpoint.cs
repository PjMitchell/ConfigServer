using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System;
using System.Collections.Generic;

namespace ConfigServer.Server
{
    internal class UploadEnpoint : IEndpoint
    {
        private readonly IHttpResponseFactory responseFactory;
        private readonly IConfigurationSetRegistry configCollection;
        private readonly ICommandBus commandBus;
        private readonly IConfigurationClientService configClientService;


        public UploadEnpoint(IHttpResponseFactory responseFactory, IConfigurationSetRegistry configCollection, ICommandBus commandBus, IConfigurationClientService configClientService)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.commandBus = commandBus;
            this.configClientService = configClientService;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // /ConfigurationSet/{clientId}/{Configuration Set}
            // POST: Uploads configuration set file
            // /Configuration/{clientId}/{Config name}
            // POST: Uploads configuration file
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();
            if (context.Request.Method != "POST" || pathParams.Length != 3)
                return;
            var client = await configClientService.GetClientOrDefault(pathParams[1]);
            if (client == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
                
            if (pathParams[0].Equals("Configuration", StringComparison.OrdinalIgnoreCase))
                await HandleUploadConfiguration(context, pathParams[2], client);
            else if (pathParams[0].Equals("ConfigurationSet", StringComparison.OrdinalIgnoreCase))
                await HandleUploadConfigurationSet(context, pathParams[2], client);
            else
                responseFactory.BuildNotFoundStatusResponse(context);
        }

        private async Task HandleUploadConfigurationSet(HttpContext context, string configSetName, ConfigurationClient client)
        {
            var configSet = configCollection.SingleOrDefault(c => configSetName.Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
            if (configSet == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var json = await context.ReadBodyTextAsync();
            var result = await commandBus.SubmitAsync(new UpdateConfigurationSetFromJsonUploadCommand(new ConfigurationIdentity(client, configCollection.GetVersion()), configSet.ConfigSetType, json));
            await responseFactory.BuildResponseFromCommandResult(context, result);
            return;
        }

        private async Task HandleUploadConfiguration(HttpContext context, string configName, ConfigurationClient client)
        {
            var configModel = configCollection.SelectMany(s => s.Configs).SingleOrDefault(c => c.Type.Name.Equals(configName, StringComparison.OrdinalIgnoreCase));
            if (configModel == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var json = await context.ReadBodyTextAsync();
            var result = await commandBus.SubmitAsync(new UpdateConfigurationFromJsonUploadCommand(new ConfigurationIdentity(client, configCollection.GetVersion()), configModel.Type, json));
            await responseFactory.BuildResponseFromCommandResult(context, result);
            return;
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, responseFactory);
            }
            else
            {
                responseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
