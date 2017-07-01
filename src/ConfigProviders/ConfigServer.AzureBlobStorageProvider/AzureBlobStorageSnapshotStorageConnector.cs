using ConfigServer.TextProvider.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.AzureBlobStorageProvider
{
    internal class AzureBlobStorageSnapshotStorageConnector : ISnapshotStorageConnector
    {
        private readonly CloudBlobClient client;
        private readonly string container;
        const string snapshotPrefix = "snapshot";
        const string indexFile = "snapshot/snapshotIndex.json";

        public AzureBlobStorageSnapshotStorageConnector(AzureBlobStorageRepositoryBuilderOptions options)
        {
            client = new CloudBlobClient(options.Uri, options.Credentials);
            container = options.Container;
        }

        public async Task<IEnumerable<SnapshotTextEntry>> GetSnapshotEntries(string snapshotId)
        {
            var entry = new List<SnapshotTextEntry>();
            var containerRef = client.GetContainerReference(container);
            foreach (var blob in await GetSnapshotConfigs(snapshotId, containerRef))
            {
                if (blob is CloudBlockBlob cloudBlockBlob)
                    entry.Add(await Map(cloudBlockBlob));
            }
            return entry;
        }

        public async Task SetSnapshotEntries(string snapshotId, IEnumerable<SnapshotTextEntry> entries)
        {
            var containerRef = client.GetContainerReference(container);
            await DeleteSnapshots(snapshotId, containerRef);
            foreach(var entry in entries)
            {
                
                var blobRef = containerRef.GetBlockBlobReference($"{snapshotPrefix}/{snapshotId}/{entry.ConfigurationName}.json");
                await blobRef.UploadTextAsync(entry.ConfigurationJson);
            }
        }

        public Task DeleteSnapshot(string snapshotId)
        {
            var containerRef = client.GetContainerReference(container);
            return DeleteSnapshots(snapshotId, containerRef);
        }

        public async Task<string> GetSnapshotRegistryFileAsync()
        {
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(indexFile);
            var exists = await entry.ExistsAsync();
            if (!exists)
                return string.Empty;
            return await entry.DownloadTextAsync();
        }

        public Task SetSnapshotRegistryFileAsync(string value)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(indexFile);
            return entry.UploadTextAsync(value);
        }

        private async Task DeleteSnapshots(string snapshotId, CloudBlobContainer container)
        {
            foreach (var entry in await GetSnapshotConfigs(snapshotId, container))
                await entry.DeleteIfExistsAsync();
        }

        private async Task<IEnumerable<CloudBlob>> GetSnapshotConfigs(string snapshotId, CloudBlobContainer containerRef)
        {
            var prefix = $"{snapshotPrefix}/{snapshotId}/";
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();
            do
            {
                var response = await containerRef.ListBlobsSegmentedAsync(prefix, continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results.OfType<CloudBlob>();
        }

        private async Task<SnapshotTextEntry> Map(CloudBlockBlob blob)
        {
            var config = await blob.DownloadTextAsync();
            var name = RemoveExtension(Helpers.TrimFolderPath(blob.Name));
            
            return new SnapshotTextEntry
            {
                ConfigurationName = name,
                ConfigurationJson = config
            };
        }

        private string RemoveExtension(string input)
        {
            var extensionStartIndex = input.LastIndexOf('.');
            return input.Remove(extensionStartIndex);
        }
    }
}
