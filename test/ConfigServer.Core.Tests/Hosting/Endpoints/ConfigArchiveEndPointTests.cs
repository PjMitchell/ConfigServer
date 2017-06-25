using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
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

        private ConfigServerOptions options;
        private static readonly Claim writeClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.AdminClaimValue);
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);

        public ConfigArchiveEndPointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            configArchive = new Mock<IConfigArchive>();
            httpResponseFactory = new Mock<IHttpResponseFactory>();
            var registry = new ConfigurationModelRegistry();
            registry.SetVersion(version);
            target = new ConfigArchiveEndPoint(configClientService.Object, registry, configArchive.Object, httpResponseFactory.Object);
            options = new ConfigServerOptions();
        }

        [Fact]
        public async Task Get_ClientArchiveSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(readClaim)
                .TestContext;
            var expectedResources = new List<ConfigArchiveEntryInfo>
            {
                new ConfigArchiveEntryInfo{ Name = "SampleConfig_123454.json"}
            };
            configArchive.Setup(r => r.GetArchiveConfigCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_Returns403_IfNoClaim()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims()
                .TestContext;
            var expectedResources = new List<ConfigArchiveEntryInfo>
            {
                new ConfigArchiveEntryInfo{ Name = "SampleConfig_123454.json"}
            };
            configArchive.Setup(r => r.GetArchiveConfigCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_GetsClientResource()
        {
            var archivedConfig = "SampleConfig_123454.json";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{archivedConfig}")
                .WithClaims(readClaim)
                .TestContext;
            var expectedResource = new ConfigArchiveEntry
            {
                HasEntry = true,
                Content = "JsonString",
                Name = archivedConfig
            };
            configArchive.Setup(r => r.GetArchiveConfig(archivedConfig, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildJsonFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_Returns403IfNoClientWriteClaim()
        {
            var archivedConfig = "SampleConfig_123454.json";
            expectedClient.ConfiguratorClaim = "ClientClaim";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{archivedConfig}")
                .WithClaims(readClaim)
                .TestContext;
            var expectedResource = new ConfigArchiveEntry
            {
                HasEntry = true,
                Content = "JsonString",
                Name = archivedConfig
            };
            configArchive.Setup(r => r.GetArchiveConfig(archivedConfig, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_ReturnsNotFoundIfNotFound()
        {
            var archivedConfig = "SampleConfig_123454.json";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{archivedConfig}")
                .WithClaims(readClaim)
                .TestContext;
            var expectedResource = new ConfigArchiveEntry
            {
                HasEntry = false
            };
            configArchive.Setup(r => r.GetArchiveConfig(archivedConfig, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
        }

        [Fact]
        public async Task Delete_ClientConfig_DeletesArchivedConfigs()
        {
            var configName = "SampleConfig_123454.json";

            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{configName}")
                .WithClaims(writeClaim)
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, options);
            configArchive.Verify(r => r.DeleteArchiveConfig(configName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Delete_Returns403_WithNoWriteClaim()
        {
            var configName = "SampleConfig_123454.json";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{configName}")
                .WithClaims(readClaim)
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, options);
            configArchive.Verify(r => r.DeleteArchiveConfig(configName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))), Times.Never);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task DeleteBefore_DeletesArchivedConfigs()
        {
            var datetime = new DateTime(2017, 02, 21);
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(writeClaim)
                .WithQueryParam("before", datetime.ToString("yyyy-MM-dd"))
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, options);
            configArchive.Verify(r => r.DeleteOldArchiveConfigs(datetime, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }
    }
}
