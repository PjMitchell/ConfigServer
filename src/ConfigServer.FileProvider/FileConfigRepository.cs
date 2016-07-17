﻿using ConfigServer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("ConfigServer.Core.Tests")]
namespace ConfigServer.FileProvider
{
    /// <summary>
    /// File store implementation of IConfigRepository
    /// </summary>
    public class FileConfigRepository : IConfigRepository
    {
        readonly string folderPath;
        readonly JsonSerializerSettings jsonSerializerSettings;
        const string indexFile = "clientIndex.json";

        internal FileConfigRepository(string folderPath, JsonSerializerSettings jsonSerializerSettings = null)
        {
            this.folderPath = folderPath;
            this.jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();
        }

        /// <summary>
        /// Creates or updates client details in store
        /// </summary>
        /// <param name="client">Updated Client detsils</param>
        /// <returns>A task that represents the asynchronous update operation.</returns>
        public Task UpdateClientAsync(ConfigurationClient client)
        {
            
            var clientLookup = GetClientStore().ToDictionary(k => k.ClientId);
            if(!clientLookup.ContainsKey(client.ClientId))
                GetFileStore().CreateSubdirectory(client.ClientId);
            clientLookup[client.ClientId] = client;
            SaveClients(clientLookup.Values);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <param name="type">Type of configuration to be retrieved</param>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Config of the type requested</returns>
        public Task<Config> GetAsync(Type type, ConfigurationIdentity id)
        {
            
            var result = ConfigFactory.CreateGenericInstance(type, id.ClientId);
            string configjson;
            if (!TryGetConfigJson(type, id.ClientId, out configjson))
                return Task.FromResult(result);
            result.SetConfiguration(JsonConvert.DeserializeObject(configjson, type, jsonSerializerSettings));
            return Task.FromResult(result);
        }

        /// <summary>
        /// Gets Configuration
        /// </summary>
        /// <typeparam name="TConfig">Type of configuration to be retrieved</typeparam>
        /// <param name="id">Identity of Configuration requested i.e which client requested the configuration</param>
        /// <returns>Config of the type requested</returns>
        public Task<Config<TConfig>> GetAsync<TConfig>(ConfigurationIdentity id) where TConfig : class, new()
        {
            string configjson;
            if (!TryGetConfigJson(typeof(TConfig), id.ClientId, out configjson))
                return Task.FromResult(new Config<TConfig> { ClientId = id.ClientId });
            var result = new Config<TConfig>
            {
                Configuration = JsonConvert.DeserializeObject<TConfig>(configjson, jsonSerializerSettings)
            };
            return Task.FromResult(result);
        }

        /// <summary>
        /// Get all Client in store
        /// </summary>
        /// <returns>Available Client</returns>
        public Task<IEnumerable<ConfigurationClient>> GetClientsAsync()
        {
            return Task.FromResult<IEnumerable<ConfigurationClient>>(GetClientStore().ToList()); 
        }

        /// <summary>
        /// Saves changes to configuration
        /// </summary>
        /// <param name="config">Updated configuration to be saved</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public Task UpdateConfigAsync(Config config)
        {
            var configPath = GetConfigPath(config.ConfigType, config.ClientId);
            File.WriteAllText(configPath, JsonConvert.SerializeObject(config.GetConfiguration(), jsonSerializerSettings));
            return Task.FromResult(true);
        }

        private bool TryGetConfigJson(Type configType, string configSetId, out string configJson)
        {
            var configPath = GetConfigPath(configType, configSetId);
            
            var result = File.Exists(configPath);
            if (result)
                configJson = File.ReadAllText(configPath);
            else
                configJson = string.Empty;
            return result;
        }

        private string GetConfigPath(Type configType, string configSetId)
        {
            var configSetFolder = GetConfigSetFolder(configSetId);
            return $"{configSetFolder.FullName}/{configType.Name}.json";
        }

        private DirectoryInfo GetConfigSetFolder(string configSetId)
        {
            return GetFileStore().EnumerateDirectories().Single(s => s.Name == configSetId);
        }

        private DirectoryInfo GetFileStore()
        {               
            return Directory.CreateDirectory(folderPath);
        }

        private IEnumerable<ConfigurationClient> GetClientStore()
        {
            var path = $"{GetFileStore().FullName}/{indexFile}";
            if (!File.Exists(path))
                return Enumerable.Empty<ConfigurationClient>();

            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<List<ConfigurationClient>>(json, jsonSerializerSettings);
        }

        private void SaveClients(ICollection<ConfigurationClient> clients)
        {
            var path = $"{GetFileStore().FullName}/{indexFile}";
            var json = JsonConvert.SerializeObject(clients, jsonSerializerSettings);
            File.WriteAllText(path, json);
        }
    }
}
