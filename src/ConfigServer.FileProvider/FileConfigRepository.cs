using ConfigServer.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.FileProvider
{
    public class FileConfigRepository : IConfigRepository
    {
        readonly string folderPath;
        readonly JsonSerializerSettings jsonSerializerSettings;

        public FileConfigRepository(string folderPath, JsonSerializerSettings jsonSerializerSettings = null)
        {
            this.folderPath = folderPath;
            this.jsonSerializerSettings = jsonSerializerSettings ?? new JsonSerializerSettings();
        }

        public Task CreateClientAsync(string configSetId)
        {
            GetFileStore().CreateSubdirectory(configSetId);
            return Task.FromResult(true);
        }

        public Task<Config> GetAsync(Type type, ConfigurationIdentity id)
        {
            
            var result = ConfigFactory.CreateGenericInstance(type, id.ClientId);
            string configjson;
            if (!TryGetConfigJson(type, id.ClientId, out configjson))
                return Task.FromResult(result);
            result.SetConfiguration(JsonConvert.DeserializeObject(configjson, type, jsonSerializerSettings));
            return Task.FromResult(result);
        }

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

        public Task<IEnumerable<string>> GetClientIdsAsync()
        {
            return Task.FromResult<IEnumerable<string>>(GetFileStore().EnumerateDirectories().Select(s => s.Name).ToList()); 
        }

        public Task SaveChangesAsync(Config config)
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
    }
}
