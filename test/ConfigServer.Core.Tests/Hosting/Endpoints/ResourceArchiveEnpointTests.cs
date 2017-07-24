using ConfigServer.Sample.Models;
using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ResourceArchiveEndpointTests
    {
        private readonly Mock<IConfigurationClientService> configClientService;
        private readonly Mock<IResourceArchive> resourceStore;
        private readonly Mock<IHttpResponseFactory> httpResponseFactory;

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private readonly Version version = new Version(1, 0);
        private readonly IEndpoint target;
        private readonly string configuratorClaim = "Configurator";

        private ConfigServerOptions options;
        private static readonly Claim writeClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.AdminClaimValue);
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);

        public ResourceArchiveEndpointTests()
        {
            expectedClient = new ConfigurationClient(clientId)
            {
                ConfiguratorClaim = configuratorClaim
            };
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            resourceStore = new Mock<IResourceArchive>();
            httpResponseFactory = new Mock<IHttpResponseFactory>();
            var registry = new ConfigurationModelRegistry();
            registry.SetVersion(version);
            target = new ResourceArchiveEndpoint(configClientService.Object, registry, resourceStore.Object, httpResponseFactory.Object);
            options = new ConfigServerOptions();
        }

        [Fact]
        public async Task Get_ClientArchiveSummary_GetsClientSummary_WithConfiguratorAndClientConfiguratorClaim()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(readClaim, new Claim(options.ClientConfiguratorClaimType, configuratorClaim))
                .TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetArchiveResourceCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_ClientArchiveSummary_GetsClientSummary_WithAdmin()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(writeClaim)
                .TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetArchiveResourceCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_Returns403_WithNoClaim()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims()
                .TestContext;
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_Returns403_WithConfiguratorAndNoClientConfigurator()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(readClaim)
                .TestContext;
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_GetsClientResource()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims(readClaim, new Claim(options.ClientConfiguratorClaimType, configuratorClaim))
                .TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = true,
                Content = new MemoryStream(),
                Name = resourceName
            };
            resourceStore.Setup(r => r.GetArchiveResource(resourceName, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_Get403IfNoClientConfiguratorClaim()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims(readClaim)
                .TestContext;
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_ReturnsNotFoundIfNotFound()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims(readClaim, new Claim(options.ClientConfiguratorClaimType, configuratorClaim))
                .TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = false
            };
            resourceStore.Setup(r => r.GetArchiveResource(resourceName, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, options);
            httpResponseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
        }

        [Fact]
        public async Task Delete_ClientResource_DeletessResources()
        {
            var resourceName = "File.txt";
            
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims(writeClaim)
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, options);
            resourceStore.Verify(r => r.DeleteArchiveResource(resourceName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Delete_Returns403_IfReadClaim()
        {
            var resourceName = "File.txt";

            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims(readClaim)
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, options);
            resourceStore.Verify(r => r.DeleteArchiveResource(resourceName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))), Times.Never);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task DeleteBefore_DeletesResources()
        {
            var datetime = new DateTime(2017, 02, 21);
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(writeClaim)
                .WithQueryParam("before",datetime.ToString("yyyy-MM-dd"))
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, options);
            resourceStore.Verify(r => r.DeleteOldArchiveResources(datetime, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

    }
}
