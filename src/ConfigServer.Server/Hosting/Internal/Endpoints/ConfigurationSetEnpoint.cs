using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ConfigServer.Server
{
    internal class ConfigurationSetEnpoint : IEndpoint
    {
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigurationSetModelPayloadMapper modelPayloadMapper;

        public ConfigurationSetEnpoint(IConfigHttpResponseFactory responseFactory, IConfigurationSetModelPayloadMapper modelPayloadMapper,  ConfigurationSetRegistry configCollection)
        {
            this.responseFactory = responseFactory;
            this.configCollection = configCollection;
            this.modelPayloadMapper = modelPayloadMapper;
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
            if (routePath.StartsWithSegments("/Model", out editRemaining))
            {
                var queryResult = configCollection.TryMatchPath(c => c.ConfigSetType.Name, remainingPath);
                if (queryResult.HasResult)
                    await responseFactory.BuildResponse(context, modelPayloadMapper.Map(queryResult.QueryResult));
                return queryResult.HasResult;
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
                Id = model.Type.Name,
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
