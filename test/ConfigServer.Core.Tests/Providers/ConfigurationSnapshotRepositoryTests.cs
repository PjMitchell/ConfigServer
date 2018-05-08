using ConfigServer.Server;
using ConfigServer.TextProvider.Core;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Providers
{
    public class ConfigurationSnapshotRepositoryTests
    {
        private Mock<ISnapshotStorageConnector> connector;
        private Mock<IConfigurationModelRegistry> registry;
        private ConfigurationIdentity defaultIdentity;
        private IConfigurationSnapshotRepository target;
        private SnapshotEntryInfo defaultEntry;

        public ConfigurationSnapshotRepositoryTests()
        {
            connector = new Mock<ISnapshotStorageConnector>();
            registry = new Mock<IConfigurationModelRegistry>();
            defaultIdentity = new ConfigurationIdentity(new ConfigurationClient("39aa8bb2-618b-4928-9c96-790f6dd5df63"), new Version(1, 0));
            defaultEntry = new SnapshotEntryInfo { Id = "39aa8bb2-218b-4928-9c96-790f6dd5df63", GroupId = "39aa8bb2-218b-4928-9c96-790f6dd5df69", Name = "1.0", TimeStamp = DateTime.UtcNow };
            target = new TextStorageSnapshotRepository(connector.Object, registry.Object);
            var json = JsonConvert.SerializeObject(new object[] { defaultEntry});
            connector.Setup(c => c.GetSnapshotRegistryFileAsync())
                .ReturnsAsync(json);
        }

        [Fact]
        public async Task Get_SnapshotRegistry()
        {
            var expected = new SnapshotEntryInfo[]
            {
                defaultEntry,
                new SnapshotEntryInfo{ Id = "99aa8bb2-218b-4928-9c96-790f6dd5df63", GroupId = "99aa8bb2-218b-4928-9c96-790f6dd5df69", Name= "1.2", TimeStamp = DateTime.UtcNow}
            };
            var json = JsonConvert.SerializeObject(expected);
            connector.Setup(c => c.GetSnapshotRegistryFileAsync())
                .ReturnsAsync(json);
            var result = await target.GetSnapshots();
            Assert.Equal(expected, result,new SnapShotEqualityComparer());
        }

        [Fact]
        public async Task Get_SnapshotObject()
        {
            var expectedInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, defaultIdentity);
            var jsonPayload = JsonConvert.SerializeObject(BuildStorageObject(expectedInstance));
            registry.Setup(r => r.GetConfigurationRegistrations(true))
                .Returns(() => new[] { new ConfigurationRegistration(typeof(SimpleConfig), typeof(SimpleConfig).Name, false) });
            
            connector.Setup(c => c.GetSnapshotEntries(defaultEntry.Id))
                .ReturnsAsync(new SnapshotTextEntry[] { new SnapshotTextEntry { ConfigurationName = typeof(SimpleConfig).Name, ConfigurationJson = jsonPayload } });
            var result = await target.GetSnapshot(defaultEntry.Id, defaultIdentity);
            Assert.Equal(1, result.Configurations.Count);
            var configuration = result.Configurations.Single().GetConfiguration() as SimpleConfig;
            Assert.NotNull(configuration);
            Assert.Equal(23, configuration.IntProperty);


        }

        [Fact]
        public async Task Get_SnapshotObjectCollection()
        {
            var expectedInstance = new ConfigCollectionInstance<SimpleConfig>(new[] { new SimpleConfig { IntProperty = 23 } }, defaultIdentity);
            var jsonPayload = JsonConvert.SerializeObject(BuildStorageObject(expectedInstance));
            registry.Setup(r => r.GetConfigurationRegistrations(true))
                .Returns(() => new[] { new ConfigurationRegistration(typeof(SimpleConfig), typeof(SimpleConfig).Name, true) });
            connector.Setup(c => c.GetSnapshotEntries(defaultEntry.Id))
                .ReturnsAsync(new SnapshotTextEntry[] { new SnapshotTextEntry { ConfigurationName = typeof(SimpleConfig).Name, ConfigurationJson = jsonPayload } });
            var result = await target.GetSnapshot(defaultEntry.Id, defaultIdentity);
            Assert.Equal(1, result.Configurations.Count);
            var configuration = result.Configurations.Single().GetConfiguration() as IEnumerable<SimpleConfig>;
            Assert.NotNull(configuration);
            var configurationAsList = configuration.ToList();
            Assert.Single(configurationAsList);
            Assert.Equal(23, configurationAsList[0].IntProperty);
        }

        [Fact]
        public async Task Get_SnapshotObjectCollection_WithInfo()
        {
            var expectedInstance = new ConfigCollectionInstance<SimpleConfig>(new[] { new SimpleConfig { IntProperty = 23 } }, defaultIdentity);
            var jsonPayload = JsonConvert.SerializeObject(BuildStorageObject(expectedInstance));
            registry.Setup(r => r.GetConfigurationRegistrations(true))
                .Returns(() => new[] { new ConfigurationRegistration(typeof(SimpleConfig), typeof(SimpleConfig).Name, true) });
            
            connector.Setup(c => c.GetSnapshotEntries(defaultEntry.Id))
                .ReturnsAsync(new SnapshotTextEntry[] { new SnapshotTextEntry { ConfigurationName = typeof(SimpleConfig).Name, ConfigurationJson = jsonPayload } });
            var result = await target.GetSnapshot(defaultEntry.Id, defaultIdentity);
            Assert.Equal(defaultEntry, result.Info, new SnapShotEqualityComparer());
        }

        [Fact]
        public async Task Set_SavesConfiguration()
        {
            var expectedInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 } , defaultIdentity);

            List<SnapshotTextEntry> observedValue = new List<SnapshotTextEntry>();
            connector.Setup(c => c.SetSnapshotEntries(defaultEntry.Id, It.IsAny<IEnumerable<SnapshotTextEntry>>()))
                .Returns((string id, IEnumerable<SnapshotTextEntry> e) =>
                {
                    observedValue = e.ToList();
                    return Task.FromResult(0);
                });
            var entry = new ConfigurationSnapshotEntry
            {
                Info = defaultEntry,
                Configurations = new List<ConfigInstance> { expectedInstance }
            };
            await target.SaveSnapshot(entry);
            Assert.Single(observedValue);
            Assert.Equal(expectedInstance.Name, observedValue[0].ConfigurationName);
            var jObject = JObject.Parse(observedValue[0].ConfigurationJson);
            Assert.Equal(defaultIdentity.ServerVersion.ToString(), jObject.GetValue(nameof(ConfigStorageObject.ServerVersion)).ToString());
            Assert.Equal(defaultIdentity.Client.ClientId, jObject.GetValue(nameof(ConfigStorageObject.ClientId)).ToString());
            Assert.Equal(expectedInstance.Name, jObject.GetValue(nameof(ConfigStorageObject.ConfigName)).ToString());
            Assert.Equal(23, jObject.GetValue(nameof(ConfigStorageObject.Config)).ToObject<SimpleConfig>().IntProperty);
        }

        [Fact]
        public async Task Set_UpdatesRegistry()
        {
            var expectedInstance = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 23 }, defaultIdentity);

            List<SnapshotEntryInfo> observedValue = new List<SnapshotEntryInfo>();
            connector.Setup(c => c.SetSnapshotRegistryFileAsync(It.IsAny<string>()))
                .Returns((string value) =>
                {
                    observedValue = JsonConvert.DeserializeObject<List<SnapshotEntryInfo>>(value);
                    return Task.FromResult(0);
                });
            var entry = new ConfigurationSnapshotEntry
            {
                Info = defaultEntry,
                Configurations = new List<ConfigInstance> { expectedInstance }
            };
            await target.SaveSnapshot(entry);
            Assert.Equal(new[] { defaultEntry }, observedValue, new SnapShotEqualityComparer());
        }

        private ConfigStorageObject BuildStorageObject(ConfigInstance config)
        {
            return new ConfigStorageObject
            {
                ServerVersion = config.ConfigurationIdentity.ServerVersion.ToString(),
                ClientId = config.ConfigurationIdentity.Client.ClientId,
                ConfigName = config.Name,
                TimeStamp = DateTime.UtcNow,
                Config = config.GetConfiguration()
            };
        }

        private class SnapShotEqualityComparer : IEqualityComparer<SnapshotEntryInfo>
        {
            public bool Equals(SnapshotEntryInfo x, SnapshotEntryInfo y)
            {
                return x.Id == y.Id && x.GroupId == y.GroupId && x.Name == y.Name && x.TimeStamp == y.TimeStamp;
            }

            public int GetHashCode(SnapshotEntryInfo obj)
            {
                return obj.Id?.GetHashCode()??0;
            }
        }

    }
}
