using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Dynamic;
using System.Linq;
using System;

namespace ConfigServer.Server
{
    internal class DownloadEndpoint : IEndpoint
    {
        readonly IHttpResponseFactory httpResponseFactory;
        readonly IConfigurationSetRegistry configCollection;
        readonly IConfigurationSetService configurationSetService;
        readonly IConfigurationClientService configClientService;
        const string jsonExtension = ".json";

        public DownloadEndpoint(IHttpResponseFactory httpResponseFactory, IConfigurationSetRegistry configCollection, IConfigurationSetService configurationSetService, IConfigurationClientService configClientService)
        {
            this.configurationSetService = configurationSetService;
            this.configCollection = configCollection;
            this.httpResponseFactory = httpResponseFactory;
            this.configClientService = configClientService;
        }

        public async Task Handle(HttpContext context, ConfigServerOptions options)
        {
            if (!CheckMethodAndAuthentication(context, options))
                return;

            // clientId/ConfigSet.json
            // clientId/ConfigSet/Config.json
            var pathParams = context.ToPathParams();
            if (pathParams.Length < 2)
            {
                httpResponseFactory.BuildNotFoundStatusResponse(context);
                return;
            }


            var client = await configClientService.GetClientOrDefault(pathParams[0]);
            if (!context.ChallengeClientConfigurator(options, client, httpResponseFactory))
                return;
            var payload = await GetPayloadOrDefault(pathParams, client);

            if (payload == null)
                httpResponseFactory.BuildNotFoundStatusResponse(context);
            else
                await httpResponseFactory.BuildJsonFileResponse(context, payload.Payload, payload.FileName);
        }

        private async Task<FilePayload> GetPayloadOrDefault(string[] pathParams, ConfigurationClient client)
        {
            if (pathParams.Length == 2)
            {
                var configurationSet = configCollection.SingleOrDefault(s => pathParams[1].Equals($"{s.ConfigSetType.Name}{jsonExtension}", StringComparison.OrdinalIgnoreCase));
                if (configurationSet == null)
                    return null;

                return new FilePayload(await GetConfigurationSet(configurationSet, client), $"{configurationSet.ConfigSetType.Name}{jsonExtension}");
            }
            if (pathParams.Length == 3)
            {
                var configurationSetModel = configCollection.SingleOrDefault(s => pathParams[1].Equals(s.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
                if (configurationSetModel == null)
                    return null;

                var configModel = configurationSetModel.Configs.SingleOrDefault(s => pathParams[2].Equals($"{s.Type.Name}{jsonExtension}", StringComparison.OrdinalIgnoreCase));
                if (configModel == null)
                    return null;
                var configurationSet = await configurationSetService.GetConfigurationSet(configurationSetModel.ConfigSetType, new ConfigurationIdentity(client, configCollection.GetVersion()));
                return new FilePayload(configModel.GetConfigurationFromConfigurationSet(configurationSet), $"{configModel.Type.Name}{jsonExtension}");
            }
            return null;
        }

        private async Task<object> GetConfigurationSet(ConfigurationSetModel model, ConfigurationClient client)
        {
            IDictionary<string, object> result = new ExpandoObject();
            var configurationSet = await configurationSetService.GetConfigurationSet(model.ConfigSetType, new ConfigurationIdentity(client, configCollection.GetVersion()));
            foreach(var configModel in model.Configs)
            {
                result[configModel.Name] = configModel.GetConfigurationFromConfigurationSet(configurationSet);
            }
            return result;
        }

        private class FilePayload
        {
            public FilePayload(object payload,string fileName)
            {
                FileName = fileName;
                Payload = payload;
            }
            public string FileName { get; }
            public object Payload { get; }
        }

        private bool CheckMethodAndAuthentication(HttpContext context, ConfigServerOptions options)
        {
            if (context.Request.Method == "GET")
            {
                return context.ChallengeUser(options.ClientAdminClaimType, new HashSet<string>(new[] { ConfigServerConstants.AdminClaimValue, ConfigServerConstants.ConfiguratorClaimValue }, StringComparer.OrdinalIgnoreCase), options.AllowAnomynousAccess, httpResponseFactory);
            }
            else
            {
                httpResponseFactory.BuildMethodNotAcceptedStatusResponse(context);
                return false; ;
            }
        }
    }
}
