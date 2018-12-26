using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System;

namespace ConfigServer.Server
{
    internal class ConfigurationSetEndpoint : IEndpoint
    {
        readonly IHttpResponseFactory httpResponseFactory;
        readonly IConfigurationModelRegistry configCollection;

        public ConfigurationSetEndpoint(IHttpResponseFactory httpResponseFactory, IConfigurationModelRegistry configCollection)
        {
            this.httpResponseFactory = httpResponseFactory;
            this.configCollection = configCollection;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            // GET: Gets all configuration set summaries
            if (!CheckMethodAndAuthentication(context, options))
                return;

            var pathParams = context.ToPathParams();

            if (pathParams.Length == 0)
            {
                await httpResponseFactory.BuildJsonResponse(context, GetConfigurationSetSummaries());
                return;
            }
            httpResponseFactory.BuildNotFoundStatusResponse(context);
            return;
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
                RequiredClientTag = model.RequiredClientTag?.Value,
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

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowManagerAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
