using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Http;
using ConfigServer.Server;
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
        private ConfigServerOptions options;
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

            defaultConfig = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 43 }, new ConfigurationIdentity(clients[0], new Version(1, 0)));
            configurationService = new Mock<IConfigurationService>();
            configurationService.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.Client == clients[0]))).ReturnsAsync(defaultConfig);

            responseFactory = new Mock<IHttpResponseFactory>();
            responseFactory.Setup(r => r.BuildJsonResponse(It.IsAny<HttpContext>(), defaultConfig.Configuration))
                .Returns(Task.FromResult(true));

            responseFactory = new Mock<IHttpResponseFactory>();
            options = new ConfigServerOptions();
            target = new ConfigEnpoint(new ConfigInstanceRouter(repository.Object, configurationService.Object, configSetConfig), responseFactory.Object);
        }

        [Fact]
        public async Task ReturnsNotFound_IfNoApplicationfound()
        {
            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithClaims()
                .TestContext;
            await target.Handle(context, options);
            responseFactory.Verify(f => f.BuildNotFoundStatusResponse(context));
        }

        [Fact]
        public async Task ReturnsNotFound_IfApplicationfound_ButNoModel()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/{clients[0].ClientId}")
               .WithClaims()
               .TestContext;
            await target.Handle(context, options);
            responseFactory.Verify(f => f.BuildNotFoundStatusResponse(context));
        }


        [Fact]
        public async Task CallsResponseFactoryWithConfig()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/{clients[0].ClientId}/{nameof(SimpleConfig)}")
               .WithClaims()
               .TestContext;
            var config = new ConfigInstance<SimpleConfig>(new SimpleConfig { IntProperty = 43 }, new ConfigurationIdentity(clients[0], new Version(1, 0)));
            configurationService.Setup(r => r.GetAsync(typeof(SimpleConfig), It.Is<ConfigurationIdentity>(arg => arg.Client.ClientId == clients[0].ClientId))).ReturnsAsync(config);
            responseFactory.Setup(r => r.BuildJsonResponse(context, config.Configuration))
                .Returns(Task.FromResult(true));

            await target.Handle(context, options);
            responseFactory.Verify(r => r.BuildJsonResponse(context, config.Configuration), Times.AtLeastOnce());
        }
    }


}
