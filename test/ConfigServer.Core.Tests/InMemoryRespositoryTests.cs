using ConfigServer.InMemoryProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class InMemoryRespositoryTests
    {
        private readonly ConfigurationCollection configurationCollection;

        public InMemoryRespositoryTests()
        {
            configurationCollection = new ConfigurationCollection();
            configurationCollection.BuildAndAddRegistration<SimpleConfig>();
        }


        [Fact]
        public void CanSaveAndRetrive()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result = inMemoryProvider.Get<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            await inMemoryProvider.SaveChangesAsync(config);
            var result = await inMemoryProvider.GetAsync<SimpleConfig>(configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public void CanSaveAndRetriveWithType()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result = (Config<SimpleConfig>)inMemoryProvider.Get(typeof(SimpleConfig),configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public void CanGetConfigSetIds()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result = inMemoryProvider.GetConfigSetIds().ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configId.ConfigSetId, result[0]);
        }

        [Fact]
        public async Task CanGetConfigSetIdsAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result =(await inMemoryProvider.GetConfigSetIdsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configId.ConfigSetId, result[0]);
        }

        [Fact]
        public void CanCreateConfigSetIds()
        {
            var configSet = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.CreateConfigSet(configSet);
            var result = inMemoryProvider.GetConfigSetIds().ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configSet, result[0]);
        }

        [Fact]
        public async Task CanCreateConfigSetIdsAsync()
        {
            var configSet = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";

            var inMemoryProvider = new InMemoryRepository();
            await inMemoryProvider.CreateConfigSetAsync(configSet);
            var result = inMemoryProvider.GetConfigSetIds().ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configSet, result[0]);
        }

        [Fact]
        public async Task Get_ReturnsNewObjectIfNotPresentAsync()
        {
            var configSet = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = configSet
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.CreateConfigSet(configSet);
            var result = await inMemoryProvider.GetAsync<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(configSet, config.ConfigSetId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }

        [Fact]
        public void Get_ReturnsNewObjectIfNotPresent()
        {
            var configSet = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var configId = new ConfigurationIdentity
            {
                ConfigSetId = configSet
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ConfigSetId = configId.ConfigSetId,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.CreateConfigSet(configSet);
            var result = inMemoryProvider.Get<SimpleConfig>(configId);
            Assert.NotNull(result);
            Assert.Equal(configSet, config.ConfigSetId);
            Assert.Equal(0, result.Configuration.IntProperty);
        }
    }
}
