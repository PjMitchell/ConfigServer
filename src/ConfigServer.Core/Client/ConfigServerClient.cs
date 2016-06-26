using ConfigServer.Core.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConfigServer.Core
{
    public class ConfigServerClient : IConfigServerClient
    {
        private readonly ConfigurationCollection collection;
        private readonly ConfigServerClientOptions options;
        private readonly IHttpClientWrapper client;

        public ConfigServerClient(IHttpClientWrapper client, ConfigurationCollection collection, ConfigServerClientOptions options)
        {
            this.client = client;
            this.collection = collection;
            this.options = options;
        }

        public async Task<object> BuildConfigAsync(Type type)
        {
            ThrowIfConfigNotRegistered(type);
            var result = await GetConfig(type.Name);
            return JsonConvert.DeserializeObject(result, type, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public async Task<TConfig> BuildConfigAsync<TConfig>() where TConfig : class, new()
        {
            ThrowIfConfigNotRegistered(typeof(TConfig));
            var result = await GetConfig(typeof(TConfig).Name);
            return JsonConvert.DeserializeObject<TConfig>(result, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<string> GetConfig(string configName)
        {
            var uri = GetUri(configName);
            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return content;

        }

        private Uri GetUri(string configName)
        {
            var baseUri = new Uri(options.ConfigServer);
            return new Uri(baseUri, $"{options.ApplicationId}/{configName}");
        }

        private void ThrowIfConfigNotRegistered(Type type)
        {
            if (!collection.Any(reg => reg.ConfigType == type))
                throw new InvalidConfigurationException(type);
        }

    }


}
