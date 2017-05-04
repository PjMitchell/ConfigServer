using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System;

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

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            // /ConfigurationSet/{clientId}/{Configuration Set}
            // POST: Uploads configuration set file
            // /Configuration/{clientId}/{Config name}
            // POST: Uploads configuration file
            var pathParams = context.ToPathParams();
            if (context.Request.Method != "POST" || pathParams.Length != 3)
                return false;
            var client = await configClientService.GetClientOrDefault(pathParams[1]);
            if (client == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return true;
            }
                
            if (pathParams[0].Equals("Configuration", StringComparison.OrdinalIgnoreCase))
                return await HandleUploadConfiguration(context, pathParams[2], client);

            if (pathParams[0].Equals("ConfigurationSet", StringComparison.OrdinalIgnoreCase))
                return await HandleUploadConfigurationSet(context, pathParams[2], client);

            responseFactory.BuildNotFoundStatusResponse(context);
            return true;
        }

        private async Task<bool> HandleUploadConfigurationSet(HttpContext context, string configSetName, ConfigurationClient client)
        {
            var configSet = configCollection.SingleOrDefault(c => configSetName.Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
            if (configSet == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return true;
            }
            var json = await context.ReadBodyTextAsync();
            var result = await commandBus.SubmitAsync(new UpdateConfigurationSetFromJsonUploadCommand(new ConfigurationIdentity(client, configCollection.GetVersion()), configSet.ConfigSetType, json));
            await responseFactory.BuildResponseFromCommandResult(context, result);
            return true;
        }

        private async Task<bool> HandleUploadConfiguration(HttpContext context, string configName, ConfigurationClient client)
        {
            var configModel = configCollection.SelectMany(s => s.Configs).SingleOrDefault(c => c.Type.Name.Equals(configName, StringComparison.OrdinalIgnoreCase));
            if (configModel == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return true;
            }
            var json = await context.ReadBodyTextAsync();
            var result = await commandBus.SubmitAsync(new UpdateConfigurationFromJsonUploadCommand(new ConfigurationIdentity(client, configCollection.GetVersion()), configModel.Type, json));
            await responseFactory.BuildResponseFromCommandResult(context, result);
            return true;
        }
    }
}
