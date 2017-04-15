using ConfigServer.Core.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using ConfigServer.Server;
using System.Runtime.CompilerServices;
using System.Linq;
using System;

namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigEndpointTests
    {
        private readonly ConfigEnpoint target;
        private readonly Mock<IConfigurationClientService> repository;
        private readonly Mock<IConfigurationService> configurationService;
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly ConfigurationSetRegistry configSetConfig;
        private readonly List<ConfigurationClient> clients;
        private readonly ConfigInstance<SimpleConfig> defaultConfig;

        public ConfigEndpointTests()
        {
            
            clients = new List<ConfigurationClient>
            {
                new ConfigurationClient { ClientId = " AplicationId-1" }
            };
            repository = new Mock<IConfigurationClientService>();
            repository.Setup(s => s.GetClientOrDefault(It.IsAny<string>()))
                .Returns((string value) => Task.FromResult(clients.SingleOrDefault(s=> string.Equals(value, s.ClientId, StringComparison.OrdinalIgnoreCase))));

            configSetConfig = new ConfigurationSetRegistry();
            var configSetDef = new ConfigurationSetModel<SimpleConfigSet>();
            configSetDef.GetOrInitialize(c=> c.Config);
            configSetConfig.AddConfigurationSet(configSetDef);

            defaultConfig = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 43 }, new ConfigurationIdentity(clients[0]));
            configurationService = new Mock<IConfigurationService>();
            configurationService.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.Client == clients[0]))).ReturnsAsync(defaultConfig);

            responseFactory = new Mock<IHttpResponseFactory>();
            responseFactory.Setup(r => r.BuildJsonResponse(It.IsAny<HttpContext>(), defaultConfig.Configuration))
                .Returns(Task.FromResult(true));

            responseFactory = new Mock<IHttpResponseFactory>();

            target = new ConfigEnpoint(new ConfigInstanceRouter(repository.Object, configurationService.Object, configSetConfig), responseFactory.Object);
        }

        [Fact]
        public async Task ReturnsFalse_IfNoApplicationfound()
        {
            var context = new TestHttpContext("/");
            var result = await target.TryHandle(context);
            Assert.False(result);
        }

        [Fact]
        public async Task ReturnsFalse_IfApplicationfound_ButNoModel()
        {
            var context = new TestHttpContext($"/{clients[0].ClientId}");
            var result = await target.TryHandle(context);
            Assert.False(result);
        }

        [Fact]
        public async Task ReturnsTrue_IfApplicationfound_ButNoModel()
        {
            var context = new TestHttpContext($"/{clients[0].ClientId}/{nameof(SimpleConfig)}");
            var result = await target.TryHandle(context);
            Assert.True(result);
        }

        [Fact]
        public async Task CallsResponseFactoryWithConfig()
        {
            var context = new TestHttpContext($"/{clients[0].ClientId}/{nameof(SimpleConfig)}");
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 43 }, new ConfigurationIdentity(clients[0]));
            configurationService.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.Client.ClientId == clients[0].ClientId))).ReturnsAsync(config);
            responseFactory.Setup(r => r.BuildJsonResponse(context, config.Configuration))
                .Returns(Task.FromResult(true));

            var result = await target.TryHandle(context);
            responseFactory.Verify(r => r.BuildJsonResponse(context, config.Configuration), Times.AtLeastOnce());
        }
    }


}
