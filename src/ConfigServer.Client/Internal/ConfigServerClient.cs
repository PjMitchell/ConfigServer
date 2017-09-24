using ConfigServer.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace ConfigServer.Client
{

    internal class ConfigServerClient : IConfigServer
    {
        private readonly IConfigurationRegistry collection;
        private readonly ConfigServerClientOptions options;
        private readonly IHttpClientWrapper client;
        private readonly IClientCachingStrategy cache;
        private readonly IClientIdProvider clientIdProvider;
        private const string cachePrefix = "ConfigServer_";

        public ConfigServerClient(IHttpClientWrapper client, IClientCachingStrategy cache,IClientIdProvider clientIdProvider, IConfigurationRegistry collection, ConfigServerClientOptions options)
        {
            this.client = client;
            this.collection = collection;
            this.options = options;
            this.cache = cache;
            this.clientIdProvider = clientIdProvider;
        }

        public Task<TConfig> GetConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetConfigAsync<TConfig>(clientIdProvider.GetCurrentClientId());
        }

        public async Task<TConfig> GetConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            return (TConfig)await GetConfigAsync(typeof(TConfig), clientId).ConfigureAwait(false);
        }

        public Task<object> GetConfigAsync(Type type)
        {
            return GetConfigAsync(type, clientIdProvider.GetCurrentClientId());
        }

        public async Task<object> GetConfigAsync(Type type, string clientId)
        {
            var registration = GetRegistration(type);
            return await cache.GetOrCreateAsync(BuildCacheKey(registration.ConfigType, clientId), () => GetConfigInternal(registration, clientId)).ConfigureAwait(false);
        }

        public Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>() where TConfig : class, new()
        {
            return GetCollectionConfigAsync<TConfig>(clientIdProvider.GetCurrentClientId());
        }

        public async Task<IEnumerable<TConfig>> GetCollectionConfigAsync<TConfig>(string clientId) where TConfig : class, new()
        {
            var type = typeof(TConfig);
            var registration = GetRegistration(type);
            return await cache.GetOrCreateAsync(BuildCacheKey(typeof(TConfig), clientId), () => GetCollectionConfigInternal<TConfig>(registration, clientId)).ConfigureAwait(false);
        }

        private async Task<object> GetConfigInternal(ConfigurationRegistration registration, string clientId)
        {
            var result = await GetConfig(registration.ConfigurationName, clientId);
            return JsonConvert.DeserializeObject(result, registration.ConfigType, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<IEnumerable<TConfig>> GetCollectionConfigInternal<TConfig>(ConfigurationRegistration registration, string clientId)
        {
            var result = await GetConfig(registration.ConfigurationName, clientId);
            return (IEnumerable<TConfig>)JsonConvert.DeserializeObject(result,typeof(List<TConfig>), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        private async Task<string> GetConfig(string configName, string clientId)
        {
            var uri = GetUri(configName, clientId);
            var response = await client.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
                throw new ConfigServerCommunicationException(uri, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            return content;

        }

        private Uri GetUri(string configName, string clientId)
        {
            return new Uri($"{options.ConfigServer}/{clientId}/{configName}");
        }

        private ConfigurationRegistration GetRegistration(Type type)
        {
            if (collection.TryGet(type, out var result))
                return result;
            throw new InvalidConfigurationException(type);
        }

        private string BuildCacheKey(Type type, string clientId) => $"{cachePrefix}{clientId}_s{type.Name}";

    }
}
