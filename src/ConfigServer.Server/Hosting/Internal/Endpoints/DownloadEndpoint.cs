using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core;
using System.Dynamic;

namespace ConfigServer.Server
{
    internal class DownloadEndpoint : IEndpoint
    {
        readonly IConfigHttpResponseFactory responseFactory;
        readonly ConfigurationSetRegistry configCollection;
        readonly IConfigRepository configRepository;
        const string jsonExtension = ".json";

        public DownloadEndpoint(IConfigHttpResponseFactory responseFactory, ConfigurationSetRegistry configCollection, IConfigRepository configRepository)
        {
            this.configRepository = configRepository;
            this.configCollection = configCollection;
            this.responseFactory = responseFactory;
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

            var clients = await configRepository.GetClientsAsync();
            var clientsResult = clients.TryMatchPath(c => c.ClientId, path);
            if (!clientsResult.HasResult)
                return null;

            var configurationSetRequestResult = configCollection.TryMatchPath(s => $"{s.ConfigSetType.Name}{jsonExtension}", clientsResult.RemainingPath);
            if (configurationSetRequestResult.HasResult && !configurationSetRequestResult.RemainingPath.HasValue)
                return new FilePayload(await GetConfigurationSet(configurationSetRequestResult.QueryResult, clientsResult.QueryResult.ClientId), $"{configurationSetRequestResult.QueryResult.ConfigSetType.Name}{jsonExtension}");

            var configurationSetResult = configCollection.TryMatchPath(s => s.ConfigSetType.Name, clientsResult.RemainingPath);
            if (!configurationSetResult.HasResult)
                return null;               

            var configModelResult = configurationSetResult.QueryResult.Configs.TryMatchPath(s => $"{s.Type.Name}{jsonExtension}", configurationSetResult.RemainingPath);
            if (!configModelResult.HasResult || configModelResult.RemainingPath.HasValue)
                return null;
            var config = await configRepository.GetAsync(configModelResult.QueryResult.Type, new ConfigurationIdentity { ClientId = clientsResult.QueryResult.ClientId });
            return new FilePayload(config.GetConfiguration(), $"{configModelResult.QueryResult.Type.Name}{jsonExtension}");
        }

        private async Task<object> GetConfigurationSet(ConfigurationSetModel model, string clientId)
        {
            IDictionary<string, object> configurationSet = new ExpandoObject();
            foreach(var configModel in model.Configs)
            {
                var config = await configRepository.GetAsync(configModel.Type, new ConfigurationIdentity { ClientId = clientId });
                configurationSet[configModel.Type.Name] = config.GetConfiguration();
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
