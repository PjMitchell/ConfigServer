using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;

namespace ConfigServer.Server
{
    internal class ConfigurationSetEnpoint : IEndpoint
    {
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigurationSetModelPayloadMapper modelPayloadMapper;
        readonly IConfigurationEditPayloadMapper configurationEditPayloadMapper;
        readonly IConfigInstanceRouter configInstanceRouter;

        public ConfigurationSetEnpoint(IConfigHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper, IConfigInstanceRouter configInstanceRouter,IConfigurationEditPayloadMapper configurationEditPayloadMapper, ConfigurationSetRegistry configCollection)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
            this.configInstanceRouter = configInstanceRouter;
            this.configurationEditPayloadMapper = configurationEditPayloadMapper;
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
                if (configInstance == null)
                    return false;
                await responseFactory.BuildResponse(context, configurationEditPayloadMapper.MapToEditConfig(configInstance, configCollection.First(s=> s.Configs.Any(a=> a.Type == configInstance.ConfigType))));
                return true;
            }
            return false;
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

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.AuthenticationOptions);
        }
    }
}
