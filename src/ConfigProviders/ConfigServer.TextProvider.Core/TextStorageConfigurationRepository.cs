using ConfigServer.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Text storage implementation of IConfigRepository
    /// </summary>
    public class TextStorageConfigurationRepository : IConfigRepository
    {

        readonly JsonSerializerSettings jsonSerializerSettings;
        readonly IStorageConnector storageConnector;

        /// <summary>
        /// Initializes File store
        /// </summary>
        public TextStorageConfigurationRepository(IStorageConnector storageConnector)
        {
            jsonSerializerSettings = new JsonSerializerSettings();
            this.storageConnector = storageConnector;
        }



        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public async Task<ConfigInstance> GetAsync(Type type, ConfigurationIdentity id)
        {
            var configId = type.Name;
            var result = ConfigFactory.CreateGenericInstance(type, id);
            var json = await storageConnector.GetConfigFileAsync(type.Name, id.Client.ClientId);

            if (!string.IsNullOrWhiteSpace(json))
                result.SetConfiguration(ConfigStorageObjectHelper.ParseConfigurationStoredObject(json, type));
            return result;
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfiguration">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>ConfigInstance of the type requested</returns>
        public async Task<ConfigInstance<TConfiguration>> GetAsync<TConfiguration>(ConfigurationIdentity id) where TConfiguration : class, new()
        {
            var result = await GetAsync(typeof(TConfiguration), id);
            return (ConfigInstance<TConfiguration>)result;
        }

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <typeparam name="TConfiguration">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        public async Task<IEnumerable<TConfiguration>> GetCollectionAsync<TConfiguration>(ConfigurationIdentity id) where TConfiguration : class, new()
        {
            var config = await GetCollectionAsync(typeof(TConfiguration), id);
            return (IEnumerable<TConfiguration>)config;
        }

        /// <summary>
        /// Gets Collection Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Enumerable of the type requested</returns>
        public async Task<IEnumerable> GetCollectionAsync(Type type, ConfigurationIdentity id)
        {
            var configId = type.Name;
            var json = await storageConnector.GetConfigFileAsync(type.Name, id.Client.ClientId);

            var configType = BuildGenericType(typeof(List<>), type);
            if (!string.IsNullOrWhiteSpace(json))
                 return (IEnumerable)ConfigStorageObjectHelper.ParseConfigurationStoredObject(json, configType);
            return (IEnumerable)Activator.CreateInstance(configType);
        }



        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public async Task UpdateConfigAsync(ConfigInstance config)
        {
            var configId = config.ConfigType.Name;
            var configText = JsonConvert.SerializeObject(ConfigStorageObjectHelper.BuildStorageObject(config), jsonSerializerSettings);
            await storageConnector.SetConfigFileAsync(configId, config.ConfigurationIdentity.Client.ClientId, configText);        
        }

        private static Type BuildGenericType(Type genericType, params Type[] typeArgs)
        {
            return genericType.MakeGenericType(typeArgs);
        }
    }
}
