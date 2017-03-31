using ConfigServer.TextProvider.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;
using System;

namespace ConfigServer.AzureBlobStorageProvider
{
    internal class StorageConnector : IStorageConnector
    {
        private readonly CloudBlobClient client;
        private readonly string container;
        const string indexFile = "clientIndex.json";
        const string indexGroupFile = "clientGroupIndex.json";


        public StorageConnector(AzureBlobStorageRepositoryBuilderOptions options)
        {
            client = new CloudBlobClient(options.Uri, options.Credentials);
            container = options.Container;
        }

        public Task<string> GetClientRegistryFileAsync()
        {
            return GetFileAsync(indexFile);
        }

        public Task<string> GetConfigFileAsync(string configId, string instanceId)
        {
            return GetFileAsync(GetConfigPath(configId,instanceId));
        }

        public Task SetClientRegistryFileAsync(string value)
        {
            return SetFileAsync(indexFile,value);
        }

        public Task SetConfigFileAsync(string configId, string instanceId, string value)
        {
            return SetFileAsync(GetConfigPath(configId, instanceId), value);
        }

        public Task<string> GetClientGroupRegistryFileAsync()
        {
            return GetFileAsync(indexGroupFile);
        }

        public Task SetClientGroupRegistryFileAsync(string value)
        {
            return SetFileAsync(indexGroupFile, value);
        }

        private async Task<string> GetFileAsync(string location)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(location);
            var exists = await entry.ExistsAsync();
            if (!exists)
                return string.Empty;
            return await entry.DownloadTextAsync();
        }

        private async Task SetFileAsync(string location, string value)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(location);
            await entry.UploadTextAsync(value);
        }

        private string GetConfigPath(string configId, string clientId) => $"{clientId}/{configId}.json";


    }
}
