using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;
using System;

namespace ConfigServer.Server
{
    internal class ConfigurationSetEnpoint : IEndpoint
    {
        readonly IHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigurationSetModelPayloadMapper modelPayloadMapper;
        readonly IConfigurationEditModelMapper configurationEditModelMapper;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigRepository configRepository;
        readonly IConfigurationClientService configClientService;
        readonly IConfigurationValidator validator;
        readonly IConfigurationUpdatePayloadMapper configurationUpdatePayloadMapper;
        readonly IEventService eventService;

        public ConfigurationSetEnpoint(IHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigInstanceRouter configInstanceRouter, IConfigurationEditModelMapper configurationEditModelMapper, IConfigurationUpdatePayloadMapper configurationUpdatePayloadMapper, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationValidator validator, IEventService eventService, IConfigurationClientService configClientService)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
            this.configInstanceRouter = configInstanceRouter;
            this.configurationEditModelMapper = configurationEditModelMapper;
            this.configRepository = configRepository;
            this.validator = validator;
            this.configurationUpdatePayloadMapper = configurationUpdatePayloadMapper;
            this.eventService = eventService;
            this.configClientService = configClientService;
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
            // Model/{ Client Id}/{ config name}
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

            if (pathParams[0].Equals("Model", StringComparison.OrdinalIgnoreCase))
            {
                var client = await configClientService.GetClientOrDefault(pathParams[1]);
                if (client == null)
                    return false;
                var configSet = configCollection.SingleOrDefault(c => pathParams[2].Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
                if (configSet == null)
                    return false;
                await responseFactory.BuildJsonResponse(context, await modelPayloadMapper.Map(configSet, new ConfigurationIdentity(client)));
                return true;
            }
            if (pathParams[0].Equals("Value", StringComparison.OrdinalIgnoreCase))
            {
                var configInstance = await configInstanceRouter.GetConfigInstanceOrDefault(pathParams[1],pathParams[2]);
                if (configInstance == null || !(context.Request.Method == "GET" || context.Request.Method == "POST"))
                    return false;
                await HandleValueRequest(context, configInstance); 
                return true;
            }
            return false;
        }

        private Task HandleValueRequest(HttpContext context,ConfigInstance configInstance)
        {
            if(context.Request.Method == "GET")
             return responseFactory.BuildJsonResponse(context, configurationEditModelMapper.MapToEditConfig(configInstance, GetConfigurationSetForModel(configInstance)));
            if (context.Request.Method == "POST")
                return HandleValuePostRequest(context, configInstance);
            throw new System.Exception("Only Get or Post methods expected");
        }

        private async Task HandleValuePostRequest(HttpContext context, ConfigInstance configInstance)
        {
            var model = GetConfigurationSetForModel(configInstance);
            ConfigInstance newConfigInstance;
            try
            {
                newConfigInstance = await GetConfigInstance(context, configInstance, model);
            }
            catch(ConfigModelParsingException ex)
            {
                await responseFactory.BuildInvalidRequestResponse(context,new []{ ex.Message});
                return;
            }
            var validationResult = await validator.Validate(newConfigInstance.GetConfiguration(), model.Get(configInstance.ConfigType), configInstance.ConfigurationIdentity);
            if (validationResult.IsValid)
            {
                await configRepository.UpdateConfigAsync(newConfigInstance);
                await eventService.Publish(new ConfigurationUpdatedEvent(newConfigInstance));
                responseFactory.BuildNoContentResponse(context);
            }
            else
            {
                await responseFactory.BuildInvalidRequestResponse(context, validationResult.Errors);
            }
        }

        private async Task<ConfigInstance> GetConfigInstance(HttpContext context, ConfigInstance configInstance, ConfigurationSetModel model)
        {
            if (configInstance.IsCollection)
            {
                var input = await context.GetJArrayFromJsonBodyAsync();
                return await configurationUpdatePayloadMapper.UpdateConfigurationInstance(configInstance, input, model);
            }
            else
            {
                var input = await context.GetJObjectFromJsonBodyAsync();
                return await configurationUpdatePayloadMapper.UpdateConfigurationInstance(configInstance, input, model);
            }
        }

        private ConfigurationSetModel GetConfigurationSetForModel(ConfigInstance configInstance)
        {
            return configCollection.First(s => s.Configs.Any(a => a.Type == configInstance.ConfigType));
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
