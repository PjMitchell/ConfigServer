using ConfigServer.Sample.Models;
using ConfigServer.Server;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class DownloadEndpointTests
    {
        private ConfigServerOptions option;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly ConfigurationSetRegistry configCollection;
        private readonly Mock<IConfigurationSetService> configurationSetService;
        private readonly Mock<IConfigurationClientService> configClientService;
        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";

        public DownloadEndpointTests()
        {
            var testconfigSet = new SampleConfigSet();
            var definition = testconfigSet.BuildConfigurationSetModel();
            configCollection = new ConfigurationSetRegistry();
            configCollection.AddConfigurationSet(definition);
            responseFactory = new Mock<IHttpResponseFactory>();
            configurationSetService = new Mock<IConfigurationSetService>();

            expectedClient = new ConfigurationClient(clientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            option = new ConfigServerOptions();
            target = new DownloadEndpoint(responseFactory.Object, configCollection, configurationSetService.Object, configClientService.Object);
        }

        [Fact]
        public async Task Get_GetsConfigAsJsonFile()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{nameof(SampleConfigSet)}/{nameof(SampleConfig)}.json")
                .WithClaims(readClaim)
                .TestContext;
            var configSet = new SampleConfigSet
            {
                SampleConfig = new Config<SampleConfig>(new SampleConfig { LlamaCapacity = 23 })
            };
            configurationSetService.Setup(s => s.GetConfigurationSet(typeof(SampleConfigSet), It.Is<ConfigurationIdentity>(i => i.Client.Equals(expectedClient))))
                .ReturnsAsync(() => configSet);
            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildJsonFileResponse(testContext, configSet.SampleConfig.Value, $"{nameof(SampleConfig)}.json"));
        }


        [Fact]
        public async Task Get_Returns403_IfNoClaim()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{nameof(SampleConfigSet)}/{nameof(SampleConfig)}.json")
                .WithClaims()
                .TestContext;
            var configSet = new SampleConfigSet
            {
                SampleConfig = new Config<SampleConfig>(new SampleConfig { LlamaCapacity = 23 })
            };
            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_Returns403_IfNoConfiguratorClaim()
        {
            expectedClient.ConfiguratorClaim = "Expected";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{nameof(SampleConfigSet)}/{nameof(SampleConfig)}.json")
                .WithClaims(readClaim)
                .TestContext;
            var configSet = new SampleConfigSet
            {
                SampleConfig = new Config<SampleConfig>(new SampleConfig { LlamaCapacity = 23 })
            };
            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_GetsConfigSetAsJsonFile()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{nameof(SampleConfigSet)}.json")
                .WithClaims(readClaim)
                .TestContext;
            var configSet = new SampleConfigSet
            {
                SampleConfig = new Config<SampleConfig>(new SampleConfig { LlamaCapacity = 23 }),
                Options = new OptionSet<Option>(new[] { new Option { Id = 1, Description = "One"}, new Option { Id = 2, Description = "Two"} }, o=> o.Id.ToString(), o=> o.Description)
            };
            configurationSetService.Setup(s => s.GetConfigurationSet(typeof(SampleConfigSet), It.Is<ConfigurationIdentity>(i => i.Client.Equals(expectedClient))))
                .ReturnsAsync(() => configSet);
            dynamic payload = null;
            responseFactory.Setup(f => f.BuildJsonFileResponse(testContext, It.IsAny<object>(), $"{nameof(SampleConfigSet)}.json"))
                .Callback((HttpContext c, object p, string n)=> payload = p)
                .Returns(()=> Task.FromResult(true));
            await target.Handle(testContext, option);
            Assert.NotNull(payload);
            var sampleConfig = payload.SampleConfig as SampleConfig;
            Assert.NotNull(sampleConfig);
            Assert.Equal(configSet.SampleConfig.Value.LlamaCapacity, sampleConfig.LlamaCapacity);
            var options = payload.Options as IEnumerable<Option>;
            Assert.NotNull(options);
            Assert.Equal(configSet.Options.Select(s=>s), options);



        }
    }
}
