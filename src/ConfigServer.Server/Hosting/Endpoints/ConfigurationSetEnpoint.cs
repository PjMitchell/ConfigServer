using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;
using System;

namespace ConfigServer.Server
{
    internal class ConfigurationSetEnpoint : IOldEndpoint
    {
        readonly IHttpResponseFactory responseFactory;
        readonly IConfigurationSetRegistry configCollection;
        readonly IConfigurationSetModelPayloadMapper modelPayloadMapper;
        readonly IConfigurationEditModelMapper configurationEditModelMapper;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigurationClientService configClientService;
        readonly ICommandBus commandBus;

        public ConfigurationSetEnpoint(IHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigInstanceRouter configInstanceRouter, IConfigurationEditModelMapper configurationEditModelMapper, IConfigurationSetRegistry configCollection, IConfigurationClientService configClientService, ICommandBus commandBus)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
            this.configInstanceRouter = configInstanceRouter;
            this.configurationEditModelMapper = configurationEditModelMapper;
            this.configClientService = configClientService;
            this.commandBus = commandBus;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            // GET: Gets all configuration set summaries
            // Model/{ Client Id}/{ Configuration Set}
            // GET: Model for configuration set
            // Value/{ Client Id}/{ config name}
            // GET: Gets Config model for editor
            // POST: Sets Config from editor model
            var pathParams = context.ToPathParams();

            if (pathParams.Length == 0)
            {
                await responseFactory.BuildJsonResponse(context, GetConfigurationSetSummaries());
                return true;
            }
            if (pathParams.Length != 3)
                return false;

            var client = await configClientService.GetClientOrDefault(pathParams[1]);
            if (client == null)
                return false;

            if (pathParams[0].Equals("Model", StringComparison.OrdinalIgnoreCase))
            {

                var configSet = configCollection.SingleOrDefault(c => pathParams[2].Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
                if (configSet == null)
                    return false;
                await responseFactory.BuildJsonResponse(context, await modelPayloadMapper.Map(configSet, new ConfigurationIdentity(client, configCollection.GetVersion())));
                return true;
            }
            if (pathParams[0].Equals("Value", StringComparison.OrdinalIgnoreCase))
            {
                switch(context.Request.Method)
                {
                    case "GET":
                        await HandleValueGetRequest(context, client, pathParams[2]);
                        break;
                    case "POST":
                        await HandleValuePostRequest(context, client, pathParams[2]);
                        break;
                    default:
                        responseFactory.BuildMethodNotAcceptedStatusResponse(context);
                        break;
                } 
                return true;
            }
            return false;
        }

        private async Task HandleValueGetRequest(HttpContext context,ConfigurationClient client, string configType)
        {
            var configInstance = await configInstanceRouter.GetConfigInstanceOrDefault(client, configType);
            if (configInstance == null)
                responseFactory.BuildNotFoundStatusResponse(context);
            else
                await responseFactory.BuildJsonResponse(context, configurationEditModelMapper.MapToEditConfig(configInstance, GetConfigurationSetForModel(configInstance)));
        }

        private async Task HandleValuePostRequest(HttpContext context, ConfigurationClient client, string configType)
        {
            var configModel = configCollection.SelectMany(s => s.Configs).SingleOrDefault(s => s.Type.Name.Equals(configType, StringComparison.OrdinalIgnoreCase));
            if(configModel == null)
            {
                responseFactory.BuildNotFoundStatusResponse(context);
                return;
            }

            var command = new UpdateConfigurationFromEditorCommand(new ConfigurationIdentity(client, configCollection.GetVersion()), configModel.Type,await context.ReadBodyTextAsync());
            var result = await commandBus.SubmitAsync(command);
            await responseFactory.BuildResponseFromCommandResult(context, result);
        }

        

        private ConfigurationSetModel GetConfigurationSetForModel(ConfigInstance configInstance)
        {
            return configCollection.GetConfigSetForConfig(configInstance.ConfigType);
        }

        private IEnumerable<ConfigurationSetSummary> GetConfigurationSetSummaries()
        {
            return configCollection.Select(MapToSummary);
        }

        private ConfigurationSetSummary MapToSummary(ConfigurationSetModel model)
        {
            return new ConfigurationSetSummary
            {
                ConfigurationSetId = model.ConfigSetType.Name,
                Name = model.Name,
                Description = model.Description,
                Configs = model.Configs.Where(w=> !w.IsReadOnly)
                    .Select(MapToSummary)
                    .ToList()
            };
        }

        private ConfigurationModelSummary MapToSummary(ConfigurationModel model)
        {
            return new ConfigurationModelSummary
            {
                Id = model.Type.Name.ToLowerCamelCase(),
                DisplayName = model.ConfigurationDisplayName,
                Description = model.ConfigurationDescription
            };
        }
    }
}
