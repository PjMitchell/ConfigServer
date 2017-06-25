using ConfigServer.Core;
using ConfigServer.Server;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigServer.TextProvider.Core
{
    internal class TextStorageSnapshotService : IConfigurationSnapshotService
    {
        private readonly ISnapshotStorageConnector connector;
        private readonly IConfigurationModelRegistry modelRegistry;
        public TextStorageSnapshotService(ISnapshotStorageConnector connector, IConfigurationModelRegistry modelRegistry)
        {
            this.connector = connector;
            this.modelRegistry = modelRegistry;
        }

        public async Task<ConfigurationSnapshotEntry> GetSnapshot(string snapshotId, ConfigurationIdentity targetConfigurationIdentity)
        {
            var snapshots = await GetSnapshots();
            var correctsnapShot = snapshots.SingleOrDefault(s => string.Equals(s.Id, snapshotId, StringComparison.OrdinalIgnoreCase));
            var configs = await connector.GetSnapshotEntries(snapshotId);
            var registrations = modelRegistry.GetConfigurationRegistrations().ToDictionary(k=> k.ConfigurationName, StringComparer.OrdinalIgnoreCase);
            var configurationInstance = BuildConfigInstances(configs, registrations, targetConfigurationIdentity).ToArray();
            return new ConfigurationSnapshotEntry { Info = correctsnapShot, Configurations = configurationInstance };
        }

        public async Task<IEnumerable<SnapshotEntryInfo>> GetSnapshots()
        {
            var json = await connector.GetSnapshotRegistryFileAsync();
            return JsonConvert.DeserializeObject<IEnumerable<SnapshotEntryInfo>>(json);
        }

        public Task SaveSnapshot(ConfigurationSnapshotEntry snapshot)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<ConfigInstance> BuildConfigInstances(IEnumerable<SnapshotTextEntry> entries, Dictionary<string, ConfigurationRegistration> configurationInRegistry, ConfigurationIdentity targetConfigurationIdentity)
        {
            foreach (var entry in entries)
            {
                if (!configurationInRegistry.TryGetValue(entry.ConfigurationName, out var configInfo))
                    continue;
                ConfigInstance newInstance = BuildInstance(targetConfigurationIdentity, entry, configInfo);
                yield return newInstance;
            }
        }

        private static ConfigInstance BuildInstance(ConfigurationIdentity targetConfigurationIdentity, SnapshotTextEntry entry, ConfigurationRegistration configInfo)
        {
            if(configInfo.IsCollection)
            {
                var collectionType = typeof(IEnumerable<>).MakeGenericType(configInfo.ConfigType);
                var newCollectionInstance = ConfigFactory.CreateGenericCollectionInstance(configInfo.ConfigType, targetConfigurationIdentity);
                newCollectionInstance.SetConfiguration(ConfigStorageObjectParser.ParseConfigurationStoredObject(entry.ConfigurationJson, collectionType));
                return newCollectionInstance;
            }
            var newInstance = ConfigFactory.CreateGenericInstance(configInfo.ConfigType, targetConfigurationIdentity);
            newInstance.SetConfiguration(ConfigStorageObjectParser.ParseConfigurationStoredObject(entry.ConfigurationJson, configInfo.ConfigType));
            return newInstance;
        }
    }
}
