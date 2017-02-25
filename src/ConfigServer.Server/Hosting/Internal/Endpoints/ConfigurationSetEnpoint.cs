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
        readonly IConfigurationEditPayloadMapper configurationEditPayloadMapper;
        readonly IConfigInstanceRouter configInstanceRouter;
        readonly IConfigRepository configRepository;
        readonly IConfigurationValidator validator;
        readonly IEventService eventService;
        public ConfigurationSetEnpoint(IConfigHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigInstanceRouter configInstanceRouter,IConfigurationEditPayloadMapper configurationEditPayloadMapper, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationValidator validator, IEventService eventService)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
            this.configInstanceRouter = configInstanceRouter;
            this.configurationEditPayloadMapper = configurationEditPayloadMapper;
            this.configRepository = configRepository;
            this.validator = validator;
            this.eventService = eventService;
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
                var clients = await configRepository.GetClientsAsync();
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
             return responseFactory.BuildResponse(context, configurationEditPayloadMapper.MapToEditConfig(configInstance, GetConfigurationSetForModel(configInstance)));
            if (context.Request.Method == "POST")
                return HandleValuePostRequest(context, configInstance);
            throw new System.Exception("Only Get or Post methods expected");
        }

        private async Task HandleValuePostRequest(HttpContext context, ConfigInstance configInstance)
        {
            var model = GetConfigurationSetForModel(configInstance);
            ConfigInstance newConfigInstance;
            if (configInstance.IsCollection)
            {
                var input = await context.GetJArrayFromJsonBodyAsync();
                newConfigInstance = await configurationEditPayloadMapper.UpdateConfigurationInstance(configInstance, input, model);
            }
            else
            {
                var input = await context.GetJObjectFromJsonBodyAsync();
                newConfigInstance = await configurationEditPayloadMapper.UpdateConfigurationInstance(configInstance, input, model);
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
                Configs = model.Configs.Select(MapToSummary).ToList()
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
