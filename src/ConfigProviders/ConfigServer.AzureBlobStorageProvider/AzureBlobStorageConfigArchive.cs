using ConfigServer.Core;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.AzureBlobStorageProvider
{
    internal class AzureBlobStorageConfigArchive : IConfigArchive
    {
        private readonly CloudBlobClient client;
        private readonly string container;

        public AzureBlobStorageConfigArchive(AzureBlobStorageRepositoryBuilderOptions options)
        {
            client = new CloudBlobClient(options.Uri, options.Credentials);
            container = options.Container;
        }

        public async Task DeleteArchiveConfig(string name, ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var location = GetArchiveConfigPath(identity, name);
            var entry = containerRef.GetBlockBlobReference(location);
            await entry.DeleteIfExistsAsync();
        }

        public async Task DeleteOldArchiveConfigs(DateTime deletionDate, ConfigurationIdentity identity)
        {
            foreach (var entry in await GetArchivedConfigs(identity))
            {
                if (entry.Properties.LastModified.HasValue && entry.Properties.LastModified.Value <= deletionDate)
                    await entry.DeleteIfExistsAsync();
            }
        }

        public async Task<ConfigArchiveEntry> GetArchiveConfig(string name, ConfigurationIdentity identity)
        {
            var location = GetArchiveConfigPath(identity, name);
            var containerRef = client.GetContainerReference(container);
            var entry = containerRef.GetBlockBlobReference(location);
            var exists = await entry.ExistsAsync();
            if (!exists)
                return new ConfigArchiveEntry();
            var storageObject = JObject.Parse(await entry.DownloadTextAsync());
            var json = storageObject.GetValue("Config").ToString();
            return new ConfigArchiveEntry
            {
                Name = name,
                HasEntry = true,
                Content = json
            };
        }

        public async Task<IEnumerable<ConfigArchiveEntryInfo>> GetArchiveConfigCatalogue(ConfigurationIdentity identity)
        {
            var entry = new List<ConfigArchiveEntryInfo>();
            foreach(var blob in await GetArchivedConfigs(identity))
            {
                if (blob is CloudBlockBlob cloudBlockBlob)
                    entry.Add(await Map(cloudBlockBlob));
            }
            return entry;
        }

        private async Task<ConfigArchiveEntryInfo> Map(CloudBlockBlob blob)
        {
            var config = JsonConvert.DeserializeObject<ConfigStorageInfo>(await blob.DownloadTextAsync());
            return new ConfigArchiveEntryInfo
            {
                Name = TrimFolderPath(blob.Name),
                Configuration = config.ConfigName,
                ServerVersion = config.ServerVersion,
                TimeStamp = config.TimeStamp,
                ArchiveTimeStamp = GetLastModifiedUtcDate(blob)
            };
        }

        private async  Task<IEnumerable<CloudBlob>> GetArchivedConfigs(ConfigurationIdentity identity)
        {
            var containerRef = client.GetContainerReference(container);
            var prefix = $"Archive/{identity.Client.ClientId}/";
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

        private string GetArchiveConfigPath(ConfigurationIdentity identity, string name)
        {
            return $"Archive/{identity.Client.ClientId}/{name}";
        }

        private static DateTime GetLastModifiedUtcDate(CloudBlob existingFile)
        {
            return existingFile.Properties.LastModified.HasValue
                ? existingFile.Properties.LastModified.Value.UtcDateTime
                : DateTime.UtcNow;
        }

        private string TrimFolderPath(string path)
        {
            var result = path;
            var index = path.IndexOf('/');
            while (index >= 0)
                result = path.Substring(index + 1);
            return result;
        }

        private class ConfigStorageInfo
        {
            public string ServerVersion { get; set; }
            public string ClientId { get; set; }
            public string ConfigName { get; set; }
            public DateTime TimeStamp { get; set; }
        }
    }
}
