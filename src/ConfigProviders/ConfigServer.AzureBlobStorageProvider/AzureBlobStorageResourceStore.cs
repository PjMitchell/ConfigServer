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

        private async Task CopyResource(CloudBlobContainer containerRef, CloudBlob sourceBlob, ConfigurationIdentity destinationIdentity)
        {
            using(var stream = await sourceBlob.OpenReadAsync())
            {
                var location = GetResourcePath(destinationIdentity.Client.ClientId, TrimFolderPath(sourceBlob.Name));
                await SetFileAsync(location, stream);
            }
        }

        public async Task DeleteResources(string name, ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var location = GetResourcePath(identity.Client.ClientId, name);
            ICloudBlob entry = await containerRef.GetBlobReferenceFromServerAsync(location);
            await entry.DeleteIfExistsAsync();
        }

        public async Task<ResourceEntry> GetResource(string name, ConfigurationIdentity identity)
        {
            var location = GetResourcePath(identity.Client.ClientId, name);
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
            var entry = (await GetResources(containerRef,identity)).Select(s=> new ResourceEntryInfo { Name = TrimFolderPath(s.Name) }).ToArray();
            return entry;
        }

        public Task UpdateResource(UpdateResourceRequest request)
        {
            var location = GetResourcePath(request.Identity.Client.ClientId, request.Name);
            return SetFileAsync(location, request.Content);
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

        private async Task SetFileAsync(string location, Stream value)
        {
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(location);
            await entry.UploadFromStreamAsync(value);
        }

        private string GetResourcePath(string clientId, string name) => $"{clientId}/{name.ToLowerInvariant()}";

        private string TrimFolderPath(string path)
        {
            var index = path.IndexOf('/');
            return index >= 0 ? path.Substring(index + 1) : path;
        }
    }
}
