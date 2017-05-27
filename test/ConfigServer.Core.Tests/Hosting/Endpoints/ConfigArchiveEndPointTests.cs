using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ConfigArchiveEndPointTests
    {
        private readonly Mock<IConfigurationClientService> configClientService;
        private readonly Mock<IConfigArchive> configArchive;
        private readonly Mock<IHttpResponseFactory> httpResponseFactory;

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private readonly Version version = new Version(1, 0);
        private readonly IEndpoint target;

        public ConfigArchiveEndPointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            configArchive = new Mock<IConfigArchive>();
            httpResponseFactory = new Mock<IHttpResponseFactory>();
            var registry = new ConfigurationSetRegistry();
            registry.SetVersion(version);
            target = new ConfigArchiveEndPoint(configClientService.Object, registry, configArchive.Object, httpResponseFactory.Object);
        }

        [Fact]
        public async Task Get_ClientArchiveSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}").TestContext;
            var expectedResources = new List<ConfigArchiveEntryInfo>
            {
                new ConfigArchiveEntryInfo{ Name = "SampleConfig_123454.json"}
            };
            configArchive.Setup(r => r.GetArchiveConfigCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_GetsClientResource()
        {
            var archivedConfig = "SampleConfig_123454.json";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{archivedConfig}").TestContext;
            var expectedResource = new ConfigArchiveEntry
            {
                HasEntry = true,
                Content = "JsonString",
                Name = archivedConfig
            };
            configArchive.Setup(r => r.GetArchiveConfig(archivedConfig, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildJsonFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_ReturnsNotFoundIfNotFound()
        {
            var archivedConfig = "SampleConfig_123454.json";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{archivedConfig}").TestContext;
            var expectedResource = new ConfigArchiveEntry
            {
                HasEntry = false
            };
            configArchive.Setup(r => r.GetArchiveConfig(archivedConfig, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
        }

        [Fact]
        public async Task Delete_ClientConfig_DeletesArchivedConfigs()
        {
            var configName = "SampleConfig_123454.json";

            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{configName}")
                .WithDelete()
                .TestContext;
            var result = await target.TryHandle(testContext);
            configArchive.Verify(r => r.DeleteArchiveConfig(configName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task DeleteBefore_DeletesArchivedConfigs()
        {
            var datetime = new DateTime(2017, 02, 21);
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithQueryParam("before", datetime.ToString("yyyy-MM-dd"))
                .WithDelete()
                .TestContext;
            var result = await target.TryHandle(testContext);
            configArchive.Verify(r => r.DeleteOldArchiveConfigs(datetime, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }
    }
}
