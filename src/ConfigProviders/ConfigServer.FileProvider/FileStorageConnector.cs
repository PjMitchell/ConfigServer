using ConfigServer.TextProvider.Core;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.FileProvider
{
    internal class FileStorageConnector : IStorageConnector
    {
        readonly string folderPath;
        const string indexFile = "clientIndex.json";

        public FileStorageConnector(FileConfigRespositoryBuilderOptions options)
        {
            folderPath = options.ConfigStorePath;
        }

        public Task<string> GetClientRegistryFileAsync()
        {
            return Task.FromResult(GetClientStore());
        }

        public Task<string> GetConfigFileAsync(string configId, string instanceId)
        {
            return Task.FromResult(GetConfigFile(configId, instanceId));
        }

        public Task SetClientRegistryFileAsync(string value)
        {
            var path = $"{GetFileStore().FullName}/{indexFile}";
            File.WriteAllText(path, value);
            return Task.FromResult(true);
        }

        public Task SetConfigFileAsync(string configId, string instanceId, string value)
        {
            var configPath = GetConfigPath(configId, instanceId);
            File.WriteAllText(configPath, value);
            return Task.FromResult(true);
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

        private string GetClientStore()
        {
            var path = $"{GetFileStore().FullName}/{indexFile}";
            if (!File.Exists(path))
                return string.Empty;

            return File.ReadAllText(path);
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
