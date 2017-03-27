using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using ConfigServer.Server.Validation;

namespace ConfigServer.Server
{
    internal class ConfigurationSetEnpoint : IEndpoint
    {
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigurationSetModelPayloadMapper modelPayloadMapper;
        readonly IConfigurationEditModelMapper configurationEditModelMapper;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigRepository configRepository;
        readonly IConfigClientRepository configClientRepository;
        readonly IConfigurationValidator validator;
        readonly IConfigurationUpdatePayloadMapper configurationUpdatePayloadMapper;
        readonly IEventService eventService;

        public ConfigurationSetEnpoint(IConfigHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigInstanceRouter configInstanceRouter, IConfigurationEditModelMapper configurationEditModelMapper, IConfigurationUpdatePayloadMapper configurationUpdatePayloadMapper, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationValidator validator, IEventService eventService, IConfigClientRepository configClientRepository)
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
            this.configClientRepository = configClientRepository;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var routePath = context.Request.Path;
            if (string.IsNullOrWhiteSpace(routePath))
            {
                await responseFactory.BuildResponse(context, GetConfigurationSetSummaries());
                return true;
            }
            PathString remainingPath;
            if (routePath.StartsWithSegments("/Model", out remainingPath))
            {
                var clients = await configClientRepository.GetClientsAsync();
                var clientResult = clients.TryMatchPath(c => c.ClientId, remainingPath);
                if (!clientResult.HasResult)
                    return false;
                var queryResult = configCollection.TryMatchPath(c => c.ConfigSetType.Name, clientResult.RemainingPath);
                if (queryResult.HasResult)
                    await responseFactory.BuildResponse(context, await modelPayloadMapper.Map(queryResult.QueryResult, new ConfigurationIdentity(clientResult.QueryResult.ClientId)));
                return queryResult.HasResult;
            }
            if (routePath.StartsWithSegments("/Value", out remainingPath))
            {
                var configInstance = await configInstanceRouter.GetConfigInstanceOrDefault(remainingPath);
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
             return responseFactory.BuildResponse(context, configurationEditModelMapper.MapToEditConfig(configInstance, GetConfigurationSetForModel(configInstance)));
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
            var validationResult = await validator.Validate(newConfigInstance.GetConfiguration(), model.Get(configInstance.ConfigType), new ConfigurationIdentity(configInstance.ClientId));
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
