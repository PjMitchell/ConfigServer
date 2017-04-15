using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;
using System.Collections.Generic;
using System;

namespace ConfigServer.Server
{
    internal class UploadEnpoint : IEndpoint
    {
        readonly IHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigRepository configRepository;
        readonly IConfigurationValidator confgiurationValidator;
        readonly IConfigurationSetUploadMapper configurationSetUploadMapper;
        readonly IEventService eventService;
        readonly IConfigurationClientService configClientService;

        public UploadEnpoint(IHttpResponseFactory responseFactory, IConfigInstanceRouter configInstanceRouter, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationValidator confgiurationValidator, IConfigurationSetUploadMapper configurationSetUploadMapper, IEventService eventService, IConfigurationClientService configClientService)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.configInstanceRouter = configInstanceRouter;
            this.configRepository = configRepository;
            this.confgiurationValidator = confgiurationValidator;
            this.configurationSetUploadMapper = configurationSetUploadMapper;
            this.eventService = eventService;
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
                return false;
            if (pathParams[0].Equals("Configuration", StringComparison.OrdinalIgnoreCase))
            {
                var configInstance = await configInstanceRouter.GetConfigInstanceOrDefault(client, pathParams[2]);
                if (configInstance == null)
                    return false;
                await HandleUploadRequest(context, configInstance);
                return true;
            }
            if (pathParams[0].Equals("ConfigurationSet", StringComparison.OrdinalIgnoreCase))
            {
                var configSet = configCollection.SingleOrDefault(c=>pathParams[2].Equals(c.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
                if (configSet == null)
                    return false;
                await HandleUploadRequest(context, client, configSet);
                return true;
            }
            return false;
        }
        private async Task HandleUploadRequest(HttpContext context, ConfigInstance configInstance)
        {
            object input;
            if(configInstance is ConfigCollectionInstance collectionInstance)
                input = await context.GetObjectFromJsonBodyOrDefaultAsync(ReflectionHelpers.BuildGenericType(typeof(IEnumerable<>), configInstance.ConfigType));
            else
                input = await context.GetObjectFromJsonBodyOrDefaultAsync(configInstance.ConfigType);
            var validationResult = await confgiurationValidator.Validate(input, GetConfigurationSetForModel(configInstance).Configs.Single(s=> s.Type == configInstance.ConfigType), configInstance.ConfigurationIdentity);
            if (validationResult.IsValid)
            {
                configInstance.SetConfiguration(input);
                await configRepository.UpdateConfigAsync(configInstance);
                await eventService.Publish(new ConfigurationUpdatedEvent(configInstance));
                responseFactory.BuildNoContentResponse(context);
            }
            else
            {
                responseFactory.BuildStatusResponse(context, 422);
            }
        }

        private async Task HandleUploadRequest(HttpContext context,ConfigurationClient client, ConfigurationSetModel configSetModel)
        {
            var input = await context.GetJObjectFromJsonBodyAsync();
            var mappedConfigs = configurationSetUploadMapper.MapConfigurationSetUpload(input, configSetModel).ToArray();
            var propertyModelLookup = configSetModel.Configs.ToDictionary(k => k.Name, StringComparer.OrdinalIgnoreCase);
            var identity = new ConfigurationIdentity(client);
            var validationResult = await ValidateConfigs(mappedConfigs, propertyModelLookup, identity);
            if (validationResult.IsValid)
            {
                foreach(var config in mappedConfigs)
                {
                    var model = propertyModelLookup[config.Key];
                    if (model.IsReadOnly)
                        continue;
                    ConfigInstance instance;
                    if(model is ConfigurationOptionModel)
                        instance = ConfigFactory.CreateGenericCollectionInstance(model.Type, identity.Client);
                    else
                        instance = ConfigFactory.CreateGenericInstance(model.Type, identity.Client);
                    instance.SetConfiguration(config.Value);
                    await configRepository.UpdateConfigAsync(instance);
                    await eventService.Publish(new ConfigurationUpdatedEvent(instance));
                }
                responseFactory.BuildNoContentResponse(context);
            }
            else
            {
                responseFactory.BuildStatusResponse(context, 422);
            }
        }

        private async Task<ValidationResult> ValidateConfigs(IEnumerable<KeyValuePair<string,object>> source, Dictionary<string, ConfigurationModel> configSetModelLookup, ConfigurationIdentity configIdentity)
        {
            var validationResults = source.Select(async kvp =>await  confgiurationValidator.Validate(kvp.Value, configSetModelLookup[kvp.Key], configIdentity)).ToArray();
            await Task.WhenAll(validationResults);
            return new ValidationResult(validationResults.Select(s=> s.Result));
        }

        private ConfigurationSetModel GetConfigurationSetForModel(ConfigInstance configInstance)
        {
            return configCollection.First(s => s.Configs.Any(a => a.Type == configInstance.ConfigType));
        }
    }
}
