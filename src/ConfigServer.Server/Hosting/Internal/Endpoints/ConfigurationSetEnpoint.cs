using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using Newtonsoft.Json;
using System.IO;

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

        public ConfigurationSetEnpoint(IConfigHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigInstanceRouter configInstanceRouter,IConfigurationEditPayloadMapper configurationEditPayloadMapper, ConfigurationSetRegistry configCollection, IConfigRepository configRepository)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
            this.configInstanceRouter = configInstanceRouter;
            this.configurationEditPayloadMapper = configurationEditPayloadMapper;
            this.configRepository = configRepository;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.AuthenticationOptions);
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
                var queryResult = configCollection.TryMatchPath(c => c.ConfigSetType.Name, remainingPath);
                if (queryResult.HasResult)
                    await responseFactory.BuildResponse(context, modelPayloadMapper.Map(queryResult.QueryResult));
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
            var input = await context.GetJObjectFromJsonBodyAsync();
            var newConfigInstance = configurationEditPayloadMapper.UpdateConfigurationInstance(configInstance,input,GetConfigurationSetForModel(configInstance));
            await configRepository.UpdateConfigAsync(newConfigInstance);
            responseFactory.BuildNoContentResponse(context);
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
