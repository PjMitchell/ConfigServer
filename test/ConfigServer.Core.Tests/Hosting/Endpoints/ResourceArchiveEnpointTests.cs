using ConfigServer.Sample.Models;
using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public ResourceArchiveEndpointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            resourceStore = new Mock<IResourceArchive>();
            httpResponseFactory = new Mock<IHttpResponseFactory>();
            var registry = new ConfigurationSetRegistry();
            registry.SetVersion(version);
            target = new ResourceArchiveEndpoint(configClientService.Object, registry, resourceStore.Object, httpResponseFactory.Object);
        }

        [Fact]
        public async Task Get_ClientArchiveSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}").TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetArchiveResourceCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_GetsClientResource()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}").TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = true,
                Content = new MemoryStream(),
                Name = resourceName
            };
            resourceStore.Setup(r => r.GetArchiveResource(resourceName, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Get_ClientArchiveResource_ReturnsNotFoundIfNotFound()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}").TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = false
            };
            resourceStore.Setup(r => r.GetArchiveResource(resourceName, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
        }

        [Fact]
        public async Task Delete_ClientResource_DeletessResources()
        {
            var resourceName = "File.txt";
            
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithDelete()
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.DeleteArchiveResource(resourceName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task DeleteBefore_DeletesResources()
        {
            var datetime = new DateTime(2017, 02, 21);
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithQueryParam("before",datetime.ToString("yyyy-MM-dd"))
                .WithDelete()
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.DeleteOldArchiveResources(datetime, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

    }
}
