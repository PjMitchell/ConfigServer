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
    internal class AzureBlobStorageResourceStore : IResourceStore
    {
        private readonly CloudBlobClient client;
        private readonly string container;

        public AzureBlobStorageResourceStore(AzureBlobStorageResourceStoreOptions options)
        {
            client = new CloudBlobClient(options.Uri, options.Credentials);
            container = options.Container;
        }

        public async Task CopyResources(ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            var containerRef = client.GetContainerReference(container);
            foreach (var blob in (await GetResources(containerRef, sourceIdentity)))
            {
                await CopyResource(containerRef, blob, destinationIdentity);
            }
        }

        public async Task CopyResources(IEnumerable<string> filesToCopy, ConfigurationIdentity sourceIdentity, ConfigurationIdentity destinationIdentity)
        {
            var files = new HashSet<string>(filesToCopy);
            var containerRef = client.GetContainerReference(container);
            foreach (var blob in (await GetResources(containerRef, sourceIdentity)).Where(s => files.Contains(TrimFolderPath(s.Name))))
            {
                await CopyResource(containerRef, blob, destinationIdentity);
            }
        }

        public async Task DeleteResources(string name, ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var location = GetResourcePath(identity, name);
            var entry = containerRef.GetBlockBlobReference(location);
            await ArchiveIfExists(containerRef, entry, identity, name);
            await entry.DeleteIfExistsAsync();
        }

        public async Task<ResourceEntry> GetResource(string name, ConfigurationIdentity identity)
        {
            var location = GetResourcePath(identity, name);
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

        public async Task<IEnumerable<ResourceEntryInfo>> GetResourceCatalogue(ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = (await GetResources(containerRef, identity)).Select(Map).ToArray();
            return entry;
        }

        private ResourceEntryInfo Map(CloudBlob blob)
        {
            return new ResourceEntryInfo { Name = TrimFolderPath(blob.Name), TimeStamp = GetLastModifiedUtcDate(blob) };
        }

        public Task UpdateResource(UpdateResourceRequest request)
        {
            return SetFileAsync(request.Identity, request.Name, request.Content);
        }

        private async Task CopyResource(CloudBlobContainer containerRef, CloudBlob sourceBlob, ConfigurationIdentity destinationIdentity)
        {
            using (var stream = await sourceBlob.OpenReadAsync())
            {
                await SetFileAsync(destinationIdentity, TrimFolderPath(sourceBlob.Name), stream);
            }
        }

        private async Task<IEnumerable<CloudBlob>> GetResources(CloudBlobContainer containerRef,ConfigurationIdentity identity)
        {
            var prefix = $"{identity.Client.ClientId}/";
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

        private async Task SetFileAsync(ConfigurationIdentity identity, string name, Stream value)
        {
            var containerRef = client.GetContainerReference(container);
            var location = GetResourcePath(identity, name);
            var entry = containerRef.GetBlockBlobReference(location);
            await ArchiveIfExists(containerRef, entry, identity, name);
            await entry.UploadFromStreamAsync(value);
        }

        private async Task ArchiveIfExists(CloudBlobContainer containerRef, CloudBlockBlob existingFile, ConfigurationIdentity identity, string name)
        {
            if (!await existingFile.ExistsAsync())
                return;
            var lastModified = GetLastModifiedUtcDate(existingFile);
            var newName = $"{Path.GetFileNameWithoutExtension(name)}_{lastModified.ToString("yyMMddHHmmssff")}{Path.GetExtension(name)}";
            var newPath = GetArchiveResourcePath(identity, newName);
            var entry = containerRef.GetBlockBlobReference(newPath);
            using (var stream = await existingFile.OpenReadAsync())
            {
                await entry.UploadFromStreamAsync(stream);
            }
        }

        private static DateTime GetLastModifiedUtcDate(CloudBlob existingFile)
        {
            return existingFile.Properties.LastModified.HasValue
                ? existingFile.Properties.LastModified.Value.UtcDateTime
                : DateTime.UtcNow;
        }

        private string GetResourcePath(ConfigurationIdentity identity, string name) => $"{identity.Client.ClientId}/{name.ToLowerInvariant()}";

        private string GetArchiveResourcePath(ConfigurationIdentity identity, string name) => $"Archive/{identity.Client.ClientId}/{name.ToLowerInvariant()}";

        private string TrimFolderPath(string path)
        {
            var index = path.IndexOf('/');
            return index >= 0 ? path.Substring(index + 1) : path;
        }
    }
}
