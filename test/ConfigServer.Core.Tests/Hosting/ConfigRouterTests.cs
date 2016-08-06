using ConfigServer.Core.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using ConfigServer.Server;
using System.Runtime.CompilerServices;


namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigRouterTests
    {
        private readonly ConfigRouter target;
        private readonly Mock<IConfigRepository> repository;
        private readonly Mock<IConfigHttpResponseFactory> responseFactory;
        private readonly ConfigurationSetRegistry configSetConfig;
        private readonly List<ConfigurationClient> clients;
        private readonly ConfigInstance<SimpleConfig> defaultConfig;

        public ConfigRouterTests()
        {
            
            clients = new List<ConfigurationClient>
            {
                new ConfigurationClient { ClientId = " AplicationId-1" }
            };
            repository = new Mock<IConfigRepository>();
            repository.Setup(s => s.GetClientsAsync())
                .ReturnsAsync(clients);

            configSetConfig = new ConfigurationSetRegistry();
            var configSetDef = new ConfigurationSetModel(typeof(ConfigurationSet));
            configSetDef.GetOrInitialize<SimpleConfig>();
            configSetConfig.AddConfigurationSet(configSetDef);

            defaultConfig = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 43 });
            repository.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.ClientId == clients[0].ClientId))).ReturnsAsync(defaultConfig);

            responseFactory = new Mock<IConfigHttpResponseFactory>();
            responseFactory.Setup(r => r.BuildResponse(It.IsAny<HttpContext>(), defaultConfig.Configuration))
                .Returns(Task.FromResult(true));

            responseFactory = new Mock<IConfigHttpResponseFactory>();

            target = new ConfigRouter(repository.Object, responseFactory.Object, configSetConfig);
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
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 43 });
            repository.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.ClientId == clients[0].ClientId))).ReturnsAsync(config);
            responseFactory.Setup(r => r.BuildResponse(context, config.Configuration))
                .Returns(Task.FromResult(true));

            var result = await target.TryHandle(context);
            responseFactory.Verify(r => r.BuildResponse(context, config.Configuration), Times.AtLeastOnce());
        }
    }


}
