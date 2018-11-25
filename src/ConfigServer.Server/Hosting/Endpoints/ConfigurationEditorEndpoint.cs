using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;
using System;

namespace ConfigServer.Server
{
    internal class ConfigurationEditorEndpoint : IEndpoint
    {
        readonly IHttpResponseFactory httpResponseFactory;
        readonly IConfigurationModelRegistry configCollection;
        readonly IConfigurationEditModelMapper configurationEditModelMapper;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigurationClientService configClientService;
        readonly ICommandBus commandBus;

        public ConfigurationEditorEndpoint(IHttpResponseFactory httpResponseFactory, IConfigInstanceRouter configInstanceRouter, IConfigurationEditModelMapper configurationEditModelMapper, IConfigurationModelRegistry configCollection, IConfigurationClientService configClientService, ICommandBus commandBus)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.configCollection = configCollection;
            this.configInstanceRouter = configInstanceRouter;
            this.configurationEditModelMapper = configurationEditModelMapper;
            this.configClientService = configClientService;
            this.commandBus = commandBus;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // /{ Client Id}/{ config name}
            // GET: Gets Config model for editor
            // POST: Sets Config from editor model
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();

            if (pathParams.Length != 2)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }                

            var client = await configClientService.GetClientOrDefault(pathParams[0]);
            if (!context.ChallengeClientConfigurator(options,client, httpResponseFactory))
                return;

            switch(context.Request.Method)
            {
                case "GET":
                    await HandleGetRequest(context, client, pathParams[1]);
                    break;
                case "POST":
                    await HandlePostRequest(context, client, pathParams[1]);
                    break;
                default:
                    httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                    break;
            } 
            return;

        }

        private async Task HandleGetRequest(HttpContext context,ConfigurationClient client, string configType)
        {
            var configInstance = await configInstanceRouter.GetConfigInstanceOrDefault(client, configType);
            if (configInstance == null)
                httpResponseFactory.BuildNotFoundStatusResponse(context);
            else
                await httpResponseFactory.BuildJsonResponse(context, configurationEditModelMapper.MapToEditConfig(configInstance, GetConfigurationModel(configInstance)));
        }

        private async Task HandlePostRequest(HttpContext context, ConfigurationClient client, string configType)
        {
            var configModel = configCollection.SelectMany(s => s.Configs).SingleOrDefault(s => s.Type.Name.Equals(configType, StringComparison.OrdinalIgnoreCase));
            if(configModel == null)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }

            var command = new UpdateConfigurationFromEditorCommand(new ConfigurationIdentity(client, configCollection.GetVersion()), configModel.Type,await context.ReadBodyTextAsync());
            var result = await commandBus.SubmitAsync(command);
            await httpResponseFactory.BuildResponseFromCommandResult(context, result);
        }        

        private ConfigurationModel GetConfigurationModel(ConfigInstance configInstance)
        {
            return configCollection.GetConfigDefinition(configInstance.ConfigType);
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET" || context.Request.Method == "POST")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowManagerAnomynousAccess, httpResponseFactory);
            }

            httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
            return false; ;

        }
    }
}
