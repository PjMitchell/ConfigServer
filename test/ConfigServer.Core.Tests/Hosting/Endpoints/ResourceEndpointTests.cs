using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ResourceEndpointTests
    {
        private readonly Mock<IConfigurationClientService> configClientService;
        private readonly Mock<IResourceStore> resourceStore;
        private readonly Mock<IHttpResponseFactory> httpResponseFactory;

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private readonly Version version = new Version(1, 0);
        private const string clientGroupImagePath = "ClientGroupImages";
        private readonly IEndpoint target;

        public ResourceEndpointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            resourceStore = new Mock<IResourceStore>();
            httpResponseFactory = new Mock<IHttpResponseFactory>();
            var registry = new ConfigurationSetRegistry();
            registry.SetVersion(version);
            target = new ResourceEndpoint(configClientService.Object, registry, resourceStore.Object, httpResponseFactory.Object);
        }

        [Fact]
        public async Task Get_ClientSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}").TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetResourceCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResources);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_ClientResource_GetsClientResource()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}").TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = true,
                Content = new MemoryStream(),
                Name = resourceName
            };
            resourceStore.Setup(r => r.GetResource(resourceName,It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Get_ClientResource_ReturnsNotFoundIfNotFound()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}").TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = false
            };
            resourceStore.Setup(r => r.GetResource(resourceName, It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
        }


        [Fact]
        public async Task Post_ClientResource_UploadsResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithPost()
                .WithFile(file, resourceName)
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.UpdateResource(It.Is<UpdateResourceRequest>(a => a.Identity.Client.Equals(expectedClient) && a.Name == resourceName && file == a.Content)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Delete_ClientResource_DeletessResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithDelete()
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.DeleteResources(resourceName, It.Is<ConfigurationIdentity>(a => a.Client.Equals(expectedClient))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Get_GroupImageSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}").TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetResourceCatalogue(It.Is<ConfigurationIdentity>(s => s.Client.ClientId.Equals(clientGroupImagePath))))
                .ReturnsAsync(() => expectedResources);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_GroupImageResource_GetsClientResource()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}/{resourceName}").TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = true,
                Content = new MemoryStream(),
                Name = resourceName
            };
            resourceStore.Setup(r => r.GetResource(resourceName, It.Is<ConfigurationIdentity>(s => s.Client.ClientId.Equals(clientGroupImagePath))))
                .ReturnsAsync(() => expectedResource);
            var result = await target.TryHandle(testContext);
            httpResponseFactory.Verify(f => f.BuildFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Post_GroupImageResource_UploadsResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}/{resourceName}")
                .WithPost()
                .WithFile(file, resourceName)
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.UpdateResource(It.Is<UpdateResourceRequest>(a => a.Identity.Client.ClientId.Equals(clientGroupImagePath) && a.Name == resourceName && file == a.Content)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Delete_GroupImageResource_DeletesResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}/{resourceName}")
                .WithDelete()
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.DeleteResources(resourceName, It.Is<ConfigurationIdentity>(a => a.Client.ClientId.Equals(clientGroupImagePath))));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }
    }
}
