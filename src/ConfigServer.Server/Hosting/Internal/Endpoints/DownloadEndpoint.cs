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
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigRepository configRepository;
        readonly IConfigurationClientService configClientService;
        const string jsonExtension = ".json";

        public DownloadEndpoint(IConfigHttpResponseFactory responseFactory, ConfigurationSetRegistry configCollection, IConfigRepository configRepository, IConfigurationClientService configClientService)
        {
            this.configRepository = configRepository;
            this.configCollection = configCollection;
            this.responseFactory = responseFactory;
            this.configClientService = configClientService;
        }

        public bool IsAuthorizated(HttpContext context, ConfigServerOptions options)
        {
            return context.CheckAuthorization(options.ManagerAuthenticationOptions);
        }

        public async Task<bool> TryHandle(HttpContext context)
        {
            var result = await GetObjectOrDefault(context.Request.Path);
            if (result == null)
                return false;
            await responseFactory.BuildJsonFileResponse(context, result.Payload, result.FileName);
            return true;
        }

        private async Task<FilePayload> GetObjectOrDefault(PathString path)
        {
            // clientId/ConfigSet.json
            // clientId/ConfigSet/Config.json
            var pathParams = path.ToPathParams();
            if (pathParams.Length < 2)
                return null;

            var client = await configClientService.GetClientOrDefault(pathParams[0]);
            if (client == null)
                return null;
            if (pathParams.Length == 2)
            {
                var configurationSet = configCollection.SingleOrDefault(s => pathParams[1].Equals($"{s.ConfigSetType.Name}{jsonExtension}", StringComparison.OrdinalIgnoreCase));
                if (configurationSet == null)
                    return null;

                return new FilePayload(await GetConfigurationSet(configurationSet, client), $"{configurationSet.ConfigSetType.Name}{jsonExtension}");
            }
            if (pathParams.Length == 3)
            {
                var configurationSet = configCollection.SingleOrDefault(s => pathParams[1].Equals(s.ConfigSetType.Name, StringComparison.OrdinalIgnoreCase));
                if (configurationSet == null)
                    return null;

                var configModel = configurationSet.Configs.SingleOrDefault(s => pathParams[2].Equals($"{s.Type.Name}{jsonExtension}", StringComparison.OrdinalIgnoreCase));
                if (configModel == null)
                    return null;
                var config = await configRepository.GetAsync(configModel.Type, new ConfigurationIdentity(client));
                return new FilePayload(config.GetConfiguration(), $"{configModel.Type.Name}{jsonExtension}");
            }
            return null;
        }

        private async Task<object> GetConfigurationSet(ConfigurationSetModel model, ConfigurationClient client)
        {
            IDictionary<string, object> configurationSet = new ExpandoObject();
            foreach(var configModel in model.Configs)
            {
                var config = await configRepository.GetAsync(configModel.Type, new ConfigurationIdentity(client));
                configurationSet[configModel.Name] = config.GetConfiguration();
            }
            return configurationSet;
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
    }
}
