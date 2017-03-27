using ConfigServer.TextProvider.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ConfigServer.FileProvider
{
    internal class FileStorageConnector : IStorageConnector
    {
        readonly string folderPath;
        const string indexFile = "clientIndex.json";
        const string groupFile = "clientGroupIndex.json";

        public FileStorageConnector(FileConfigRespositoryBuilderOptions options)
        {
            folderPath = options.ConfigStorePath;
        }


        public Task<string> GetClientRegistryFileAsync()
        {
            return Task.FromResult(GetStore(indexFile));
        }

        public Task<string> GetConfigFileAsync(string configId, string instanceId)
        {
            return Task.FromResult(GetConfigFile(configId, instanceId));
        }

        public Task SetClientRegistryFileAsync(string value)
        {
            return SetRegistryFileAsynx(value, indexFile);
        }



        public Task SetConfigFileAsync(string configId, string instanceId, string value)
        {
            var configPath = GetConfigPath(configId, instanceId);
            File.WriteAllText(configPath, value);
            return Task.FromResult(true);
        }

        public Task<string> GetClientGroupRegistryFileAsync()
        {
            return Task.FromResult(GetStore(groupFile));
        }

        public Task SetClientGroupRegistryFileAsync(string value)
        {
            return SetRegistryFileAsynx(value, groupFile);
        }

        private string GetConfigFile(string configId, string instanceId)
        {
            var configPath = GetConfigPath(configId, instanceId);
            var result = File.Exists(configPath);
            if (result)
                return File.ReadAllText(configPath);
            else
                return string.Empty;
        }

        private string GetStore(string storeFile)
        {
            var path = $"{GetFileStore().FullName}/{storeFile}";
            if (!File.Exists(path))
                return string.Empty;

            return File.ReadAllText(path);
        }

        private Task SetRegistryFileAsynx(string file, string storeFileLocation)
        {
            var path = $"{GetFileStore().FullName}/{storeFileLocation}";
            File.WriteAllText(path, file);
            return Task.FromResult(true);
        }

        private DirectoryInfo GetFileStore()
        {
            return Directory.CreateDirectory(folderPath);
        }

        private DirectoryInfo GetConfigSetFolder(string clientId)
        {
            var filestore = GetFileStore();
            return filestore.EnumerateDirectories().SingleOrDefault(s => s.Name == clientId) ?? filestore.CreateSubdirectory(clientId);
        }

        private string GetConfigPath(string configId, string clientId)
        {
            var configSetFolder = GetConfigSetFolder(clientId);
            return $"{configSetFolder.FullName}/{configId}.json";
        }


    }
}
