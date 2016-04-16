using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using ConfigServer.InMemoryProvider;
using ConfigServer.Infrastructure;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigServerBuilderExtensionTests
    {
        private readonly IServiceCollection serviceCollection;

        public ConfigServerBuilderExtensionTests()
        {
            serviceCollection = new ServiceCollection();
        }

        [Fact]
        public void CanSetUpSimpleInMemoryConfig()
        {
            var config = new SimpleConfig { IntProperty = 23 };
            var applicationId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            serviceCollection.UseConfigServer()
                .UseInMemoryProvider()
                .UseLocalConfigServer(applicationId)
                .WithConfig<SimpleConfig>();
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var configRepo = serviceProvider.GetService<IConfigRepository>();
            configRepo.SaveChanges(new Config<SimpleConfig> { ConfigSetId = applicationId, Configuration = config });
            var configFromServer = serviceProvider.GetService<SimpleConfig>();
            Assert.Equal(config.IntProperty, configFromServer.IntProperty);


        }

        [Fact]
        public void CanAddConfigRegistry()
        {
            var config = new SimpleConfig { IntProperty = 23 };
            var applicationId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var builder = serviceCollection.UseConfigServer()
                .UseInMemoryProvider()
                .UseLocalConfigServer(applicationId)
                .WithConfig<SimpleConfig>();
            var regs = builder.ConfigurationCollection.ToList();
            Assert.Equal(1, regs.Count);
            Assert.Equal(typeof(SimpleConfig).Name, regs[0].ConfigurationName);
        }

        [Fact]
        public void ConfigRegistry_IsAdded()
        {
            var config = new SimpleConfig { IntProperty = 23 };
            var applicationId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
            var builder = serviceCollection.UseConfigServer()
                .UseInMemoryProvider()
                .UseLocalConfigServer(applicationId)
                .WithConfig<SimpleConfig>();
            var serviceProvider = builder.ServiceCollection.BuildServiceProvider();
            var configRepo = serviceProvider.GetRequiredService<ConfigurationCollection>();
            var regs = configRepo.ToList();
            Assert.Equal(1, regs.Count);
            Assert.Equal(typeof(SimpleConfig).Name, regs[0].ConfigurationName);
        }
    }
}
