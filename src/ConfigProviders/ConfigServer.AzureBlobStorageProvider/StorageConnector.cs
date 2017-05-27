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

        public async Task SetConfigFileAsync(string configId, string instanceId, string value)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(GetConfigPath(configId, instanceId));
            await ArchiveIfExists(configId, instanceId, containerRef, entry);
            await entry.UploadTextAsync(value);            
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

        private async Task ArchiveIfExists(string configId, string instanceId, CloudBlobContainer containerRef, CloudBlockBlob existingFile)
        {
            if (!await existingFile.ExistsAsync())
                return;
            
            var newPath = GetArchiveConfigPath(configId, instanceId);
            var entry = containerRef.GetBlockBlobReference(newPath);
            using (var stream = await existingFile.OpenReadAsync())
            {
                await entry.UploadFromStreamAsync(stream);
            }
        }

        private string GetConfigPath(string configId, string clientId) => $"{clientId}/{configId}.json";

        private string GetArchiveConfigPath(string configId, string clientId)
        {
            return $"Archive/{clientId}/{configId}_{DateTime.UtcNow.ToString("yyMMddHHmmssff")}.json";
        }
    }
}
