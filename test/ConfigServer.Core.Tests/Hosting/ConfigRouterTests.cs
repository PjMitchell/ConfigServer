using ConfigServer.Core.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using ConfigServer.Core.Internal;

namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigRouterTests
    {
        private readonly ConfigRouter target;
        private readonly Mock<IConfigRepository> repository;
        private readonly Mock<IConfigHttpResponseFactory> responseFactory;
        private readonly ConfigurationSetRegistry configSetConfig;
        private readonly List<string> configSetIds;
        private readonly Config<SimpleConfig> defaultConfig;

        public ConfigRouterTests()
        {
            
            configSetIds = new List<string>
            {
                "AplicationId-1"
            };
            repository = new Mock<IConfigRepository>();
            repository.Setup(s => s.GetConfigSetIdsAsync())
                .ReturnsAsync(configSetIds);

            configSetConfig = new ConfigurationSetRegistry();
            var configSetDef = new ConfigurationSetModel(typeof(ConfigurationSet));
            configSetDef.GetOrInitialize<SimpleConfig>();
            configSetConfig.AddConfigurationSet(configSetDef);

            defaultConfig = new Config<SimpleConfig>(new SimpleConfig { IntProperty = 43 });
            repository.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.ConfigSetId == configSetIds[0]))).ReturnsAsync(defaultConfig);

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
            var context = new TestHttpContext($"/{configSetIds[0]}");
            var result = await target.TryHandle(context);
            Assert.False(result);
        }

        [Fact]
        public async Task ReturnsTrue_IfApplicationfound_ButNoModel()
        {
            var context = new TestHttpContext($"/{configSetIds[0]}/{nameof(SimpleConfig)}");
            var result = await target.TryHandle(context);
            Assert.True(result);
        }

        [Fact]
        public async Task CallsResponseFactoryWithConfig()
        {
            var context = new TestHttpContext($"/{configSetIds[0]}/{nameof(SimpleConfig)}");
            var config = new Config<SimpleConfig>(new SimpleConfig { IntProperty = 43 });
            repository.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.ConfigSetId == configSetIds[0]))).ReturnsAsync(config);
            responseFactory.Setup(r => r.BuildResponse(context, config.Configuration))
                .Returns(Task.FromResult(true));

            var result = await target.TryHandle(context);
            responseFactory.Verify(r => r.BuildResponse(context, config.Configuration), Times.AtLeastOnce());
        }
    }


}
