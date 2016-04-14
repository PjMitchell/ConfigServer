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

        [Fact]
        public void CanSaveAndRetrive()
        {
            var configId = new ConfigurationIdentity
            {
                ApplicationIdentity = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ApplicationIdentity = configId.ApplicationIdentity,
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
                ApplicationIdentity = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            const int testValue = 23;
            var config = new Config<SimpleConfig>
            {
                ApplicationIdentity = configId.ApplicationIdentity,
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
                ApplicationIdentity = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            var name = "ConfigName";
            const int testValue = 23;
            var config = new Config<SimpleConfig>(name)
            {
                ApplicationIdentity = configId.ApplicationIdentity,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result = (Config<SimpleConfig>)inMemoryProvider.Get(typeof(SimpleConfig),configId);
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public async Task CanSaveAndRetriveWithNameAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ApplicationIdentity = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            var name = "ConfigName";
            const int testValue = 23;
            var config = new Config<SimpleConfig>(name)
            {
                ApplicationIdentity = configId.ApplicationIdentity,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            await inMemoryProvider.SaveChangesAsync(config);
            var result =  (Config<SimpleConfig>)(await inMemoryProvider.GetAsync(typeof(SimpleConfig), configId));
            Assert.Equal(testValue, result.Configuration.IntProperty);
        }

        [Fact]
        public void CanGetApplicationIds()
        {
            var configId = new ConfigurationIdentity
            {
                ApplicationIdentity = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            var name = "ConfigName";
            const int testValue = 23;
            var config = new Config<SimpleConfig>(name)
            {
                ApplicationIdentity = configId.ApplicationIdentity,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result = inMemoryProvider.GetApplicationIds().ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configId.ApplicationIdentity, result[0]);
        }

        [Fact]
        public async Task CanGetApplicationIdsAsync()
        {
            var configId = new ConfigurationIdentity
            {
                ApplicationIdentity = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4"
            };
            var name = "ConfigName";
            const int testValue = 23;
            var config = new Config<SimpleConfig>(name)
            {
                ApplicationIdentity = configId.ApplicationIdentity,
                Configuration = new SimpleConfig { IntProperty = testValue }
            };
            var inMemoryProvider = new InMemoryRepository();
            inMemoryProvider.SaveChanges(config);
            var result =(await inMemoryProvider.GetApplicationIdsAsync()).ToList();
            Assert.Equal(1, result.Count);
            Assert.Equal(configId.ApplicationIdentity, result[0]);
        }

    }
}
