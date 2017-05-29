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
        private ConfigurationClient expectedTargetClient;

        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private const string targetClientId = "9E37AC18-A00F-47A5-B84E-C79E0823F6D2";

        private readonly Version version = new Version(1, 0);
        private const string clientGroupImagePath = "ClientGroupImages";
        private readonly IOldEndpoint target;

        public ResourceEndpointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            expectedTargetClient = new ConfigurationClient(targetClientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            configClientService.Setup(s => s.GetClientOrDefault(targetClientId))
                .ReturnsAsync(() => expectedTargetClient);
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
            resourceStore.Setup(r => r.GetResourceCatalogue(ItMatchesClient(expectedClient)))
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
            resourceStore.Setup(r => r.GetResource(resourceName, ItMatchesClient(expectedClient)))
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
            resourceStore.Setup(r => r.GetResource(resourceName, ItMatchesClient(expectedClient)))
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
            resourceStore.Verify(r => r.DeleteResources(resourceName, ItMatchesClient(expectedClient)));
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
            resourceStore.Setup(r => r.GetResourceCatalogue(ItMatchesClientId(clientGroupImagePath)))
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
            resourceStore.Setup(r => r.GetResource(resourceName, ItMatchesClientId(clientGroupImagePath)))
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
            resourceStore.Verify(r => r.DeleteResources(resourceName, ItMatchesClientId(clientGroupImagePath)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task CopyTo_CopiesResources()
        {
            var resourceName = "File.txt";
            var expected = new HashSet<string>(new[] { resourceName });
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/to/{targetClientId}")
                .WithPost()
                .WithJsonBody(new[] { resourceName })
                .TestContext;
            var result = await target.TryHandle(testContext);
            resourceStore.Verify(r => r.CopyResources(It.Is<IEnumerable<string>>(s=> expected.SetEquals(s)), ItMatchesClient(expectedClient),ItMatchesClient(expectedTargetClient)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        private static ConfigurationIdentity ItMatchesClientId(string expectedClientId)
        {
            return It.Is<ConfigurationIdentity>(s => s.Client.ClientId.Equals(expectedClientId));
        }

        private static ConfigurationIdentity ItMatchesClient(ConfigurationClient expectedClient)
        {
            return It.Is<ConfigurationIdentity>(s => s.Client.Equals(expectedClient));
        }
    }
}
