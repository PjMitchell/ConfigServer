using ConfigServer.Core;
using ConfigServer.Server;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigServer.TextProvider.Core
{
    /// <summary>
    /// Text storage Snapshot Service
    /// </summary>
    public class TextStorageSnapshotRepository : IConfigurationSnapshotRepository
    {
        private readonly ISnapshotStorageConnector connector;
        private readonly IConfigurationModelRegistry modelRegistry;
        private readonly static SemaphoreSlim locker = new SemaphoreSlim(1);

        /// <summary>
        /// contructs new TextStorageSnapshotRepository
        /// </summary>
        /// <param name="connector">Connector for TextStorageSnapshotRepository</param>
        /// <param name="modelRegistry">Model registry for app</param>
        public TextStorageSnapshotRepository(ISnapshotStorageConnector connector, IConfigurationModelRegistry modelRegistry)
        {
            this.connector = connector;
            this.modelRegistry = modelRegistry;
        }

        /// <summary>
        /// Get snapshot by id
        /// </summary>
        /// <param name="snapshotId">snapshot id</param>
        /// <param name="targetConfigurationIdentity">snapshot id</param>
        /// <returns>Snapshot entry for Id</returns>
        public async Task<ConfigurationSnapshotEntry> GetSnapshot(string snapshotId, ConfigurationIdentity targetConfigurationIdentity)
        {
            var snapshots = await GetSnapshots();
            var correctsnapShot = snapshots.SingleOrDefault(s => string.Equals(s.Id, snapshotId, StringComparison.OrdinalIgnoreCase));
            var configs = await connector.GetSnapshotEntries(snapshotId);
            var registrations = modelRegistry.GetConfigurationRegistrations(true).ToDictionary(k=> k.ConfigurationName, StringComparer.OrdinalIgnoreCase);
            var configurationInstance = BuildConfigInstances(configs, registrations, targetConfigurationIdentity).ToArray();
            return new ConfigurationSnapshotEntry { Info = correctsnapShot, Configurations = configurationInstance };
        }

        /// <summary>
        /// Get all snapshots
        /// </summary>
        /// <returns>Summary of all snapshots</returns>
        public async Task<IEnumerable<SnapshotEntryInfo>> GetSnapshots()
        {
            var json = await connector.GetSnapshotRegistryFileAsync();
            return JsonConvert.DeserializeObject<IEnumerable<SnapshotEntryInfo>>(json)?? Enumerable.Empty<SnapshotEntryInfo>();
        }


        /// <summary>
        /// Save snapshot
        /// </summary>
        /// <param name="snapshot">snapshot to saved</param>
        /// <returns>Task for operation</returns>
        public async Task SaveSnapshot(ConfigurationSnapshotEntry snapshot)
        {
            var entries = snapshot.Configurations.Select(Map);
            await connector.SetSnapshotEntries(snapshot.Info.Id, entries);
            
            await locker.WaitAsync();
            try
            {
                var snapshots = (await GetSnapshots()).ToDictionary(k=> k.Id, StringComparer.OrdinalIgnoreCase);
                snapshots[snapshot.Info.Id] = snapshot.Info;
                await connector.SetSnapshotRegistryFileAsync(JsonConvert.SerializeObject(snapshots.Values));

            }
            finally
            {
                locker.Release();
            }
        }

        /// <summary>
        /// Deletes snapshot
        /// </summary>
        /// <param name="snapshotId">snapshot id</param>
        /// <returns>Task for operation</returns>
        public async Task DeleteSnapshot(string snapshotId)
        {
            await locker.WaitAsync();
            try
            {
                var snapshots = (await GetSnapshots()).Where(k => string.Equals(k.Id,snapshotId, StringComparison.OrdinalIgnoreCase));
                await connector.SetSnapshotRegistryFileAsync(JsonConvert.SerializeObject(snapshots));
            }
            finally
            {
                locker.Release();
            }
            await connector.DeleteSnapshot(snapshotId);
        }


        private SnapshotTextEntry Map(ConfigInstance config)
        {
            var configObject = ConfigStorageObjectHelper.BuildStorageObject(config);
            return  new SnapshotTextEntry { ConfigurationJson = JsonConvert.SerializeObject(configObject), ConfigurationName = config.Name } ;
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
                newCollectionInstance.SetConfiguration(ConfigStorageObjectHelper.ParseConfigurationStoredObject(entry.ConfigurationJson, collectionType));
                return newCollectionInstance;
            }
            var newInstance = ConfigFactory.CreateGenericInstance(configInfo.ConfigType, targetConfigurationIdentity);
            newInstance.SetConfiguration(ConfigStorageObjectHelper.ParseConfigurationStoredObject(entry.ConfigurationJson, configInfo.ConfigType));
            return newInstance;
        }

    }
}
