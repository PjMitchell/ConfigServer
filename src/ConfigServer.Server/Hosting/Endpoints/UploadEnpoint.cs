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
        private readonly IHttpResponseFactory httpResponseFactory;
        private readonly IConfigurationSetRegistry configCollection;
        private readonly ICommandBus commandBus;
        private readonly IConfigurationClientService configClientService;
        private readonly IUploadToEditorModelMapper uploadToEditorModelMapper;

        public UploadEnpoint(IHttpResponseFactory httpResponseFactory, IConfigurationSetRegistry configCollection, ICommandBus commandBus, IConfigurationClientService configClientService, IUploadToEditorModelMapper uploadToEditorModelMapper)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.configCollection = configCollection;
            this.commandBus = commandBus;
            this.configClientService = configClientService;
            this.uploadToEditorModelMapper = uploadToEditorModelMapper;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // /ConfigurationSet/{clientId}/{Configuration Set}
            // POST: Uploads configuration set file
            // /Configuration/{clientId}/{Config name}
            // POST: Uploads configuration file
            // /Editor/{clientId}/{Config name}
            // POST: Uploads configuration file
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();
            if (pathParams.Length != 3)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
                
            var client = await configClientService.GetClientOrDefault(pathParams[1]);
            if (!context.ChallengeClientConfigurator(options, client, httpResponseFactory))
                return;
            
            if (pathParams[0].Equals("Configuration", StringComparison.OrdinalIgnoreCase))
                await HandleUploadConfiguration(context, pathParams[2], client);
            else if (pathParams[0].Equals("ConfigurationSet", StringComparison.OrdinalIgnoreCase))
                await HandleUploadConfigurationSet(context, pathParams[2], client);
            else if (pathParams[0].Equals("Editor", StringComparison.OrdinalIgnoreCase))
                await HandleUploadToEditor(context, pathParams[2], client);
            else
                httpResponseFactory.BuildNotFoundStatusResponse(context);
        }

        private async Task HandleUploadConfigurationSet(HttpContext context, string configSetName, ConfigurationClient client)
        {
            var configSet = configCollection.SingleOrDefault(c => configSetName.Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
            if (configSet == null)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var json = await context.ReadBodyTextAsync();
            var result = await commandBus.SubmitAsync(new UpdateConfigurationSetFromJsonUploadCommand(BuildIdentity(client), configSet.ConfigSetType, json));
            await httpResponseFactory.BuildResponseFromCommandResult(context, result);
            return;
        }

        private async Task HandleUploadConfiguration(HttpContext context, string configName, ConfigurationClient client)
        {
            var configModel = configCollection.SelectMany(s => s.Configs).SingleOrDefault(c => c.Type.Name.Equals(configName, StringComparison.OrdinalIgnoreCase));
            if (configModel == null)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var json = await context.ReadBodyTextAsync();
            var result = await commandBus.SubmitAsync(new UpdateConfigurationFromJsonUploadCommand(BuildIdentity(client), configModel.Type, json));
            await httpResponseFactory.BuildResponseFromCommandResult(context, result);
            return;
        }

        private async Task HandleUploadToEditor(HttpContext context, string configName, ConfigurationClient client)
        {
            var configModel = configCollection.SelectMany(s => s.Configs).SingleOrDefault(c => c.Type.Name.Equals(configName, StringComparison.OrdinalIgnoreCase));
            if (configModel == null)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }
            var json = await context.ReadBodyTextAsync();
            var result = uploadToEditorModelMapper.MapUploadToEditModel(json, BuildIdentity(client), configModel);
            await httpResponseFactory.BuildJsonResponse(context, result);
            return;
        }

        private ConfigurationIdentity BuildIdentity(ConfigurationClient client) => new ConfigurationIdentity(client, configCollection.GetVersion());


        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }

            httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
            return false;

        }
    }
}
