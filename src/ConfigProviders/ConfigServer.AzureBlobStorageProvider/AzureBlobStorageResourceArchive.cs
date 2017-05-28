using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigServer.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;

namespace ConfigServer.AzureBlobStorageProvider
{
    internal class AzureBlobStorageResourceArchive : IResourceArchive
    {
        private readonly CloudBlobClient client;
        private readonly string container;

        public AzureBlobStorageResourceArchive(AzureBlobStorageResourceStoreOptions options)
        {
            client = new CloudBlobClient(options.Uri, options.Credentials);
            container = options.Container;
        }

        public async Task<IEnumerable<ResourceEntryInfo>> GetArchiveResourceCatalogue(ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = (await GetResources(containerRef, identity)).Select(Map).ToArray();
            return entry;
        }

        public async Task<ResourceEntry> GetArchiveResource(string name, ConfigurationIdentity identity)
        {
            var location = GetArchiveResourcePath(identity, name);
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(location);
            var exists = await entry.ExistsAsync();
            if (!exists)
                return new ResourceEntry();
            var stream = await entry.OpenReadAsync();

            return new ResourceEntry
            {
                Name = name,
                HasEntry = true,
                Content = stream
            };
        }

        public async Task DeleteArchiveResource(string name, ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var location = GetArchiveResourcePath(identity, name);
            var entry = containerRef.GetBlockBlobReference(location);
            await entry.DeleteIfExistsAsync();
        }

        public async Task DeleteOldArchiveResources(DateTime deletionDate, ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            foreach(var entry in await GetResources(containerRef, identity))
            {
                if (entry.Properties.LastModified.HasValue && entry.Properties.LastModified.Value <= deletionDate)
                    await entry.DeleteIfExistsAsync();
            }
            
        }

        private async Task<IEnumerable<CloudBlob>> GetResources(CloudBlobContainer containerRef,ConfigurationIdentity identity)
        {
            var prefix = $"Archive/{identity.Client.ClientId}/";
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();
            do
            {
                var response = await containerRef.ListBlobsSegmentedAsync(prefix,continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);
            return results.OfType<CloudBlob>();

        }

        private static DateTime GetLastModifiedUtcDate(CloudBlob existingFile)
        {
            return existingFile.Properties.LastModified.HasValue
                ? existingFile.Properties.LastModified.Value.UtcDateTime
                : DateTime.UtcNow;
        }

        private ResourceEntryInfo Map(CloudBlob blob)
        {
            return new ResourceEntryInfo { Name = Helpers.TrimFolderPath(blob.Name), TimeStamp = GetLastModifiedUtcDate(blob) };
        }

        private string GetArchiveResourcePath(ConfigurationIdentity identity, string name) => $"Archive/{identity.Client.ClientId}/{name.ToLowerInvariant()}";
    }
}
