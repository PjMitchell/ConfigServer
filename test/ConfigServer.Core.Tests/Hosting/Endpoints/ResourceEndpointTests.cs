using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
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
        private ConfigServerOptions option;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;

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
            option = new ConfigServerOptions();
            target = new ResourceEndpoint(configClientService.Object, registry, resourceStore.Object, httpResponseFactory.Object);
        }

        [Fact]
        public async Task Get_ClientSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims()
                .TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetResourceCatalogue(ItMatchesClient(expectedClient)))
                .ReturnsAsync(() => expectedResources);
            await target.Handle(testContext, option);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_ClientResource_GetsClientResource()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims()
                .TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = true,
                Content = new MemoryStream(),
                Name = resourceName
            };
            resourceStore.Setup(r => r.GetResource(resourceName, ItMatchesClient(expectedClient)))
                .ReturnsAsync(() => expectedResource);

            await target.Handle(testContext, option);
            httpResponseFactory.Verify(f => f.BuildFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }

        [Fact]
        public async Task Get_ClientResource_ReturnsNotFoundIfNotFound()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithClaims()
                .TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = false
            };
            resourceStore.Setup(r => r.GetResource(resourceName, ItMatchesClient(expectedClient)))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, option);
            httpResponseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
        }


        [Fact]
        public async Task Post_ClientResource_UploadsResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithPost()
                .WithClaims(readClaim)
                .WithFile(file, resourceName)
                .TestContext;
            await target.Handle(testContext, option);
            resourceStore.Verify(r => r.UpdateResource(It.Is<UpdateResourceRequest>(a => a.Identity.Client.Equals(expectedClient) && a.Name == resourceName && file == a.Content)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Post_Returns403_IfNoClientReadClaim()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithPost()
                .WithClaims()
                .WithFile(file, resourceName)
                .TestContext;
            await target.Handle(testContext, option);
            resourceStore.Verify(r => r.UpdateResource(It.IsAny<UpdateResourceRequest>()), Times.Never);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Delete_ClientResource_DeletessResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithDelete()
                .WithClaims(readClaim)
                .TestContext;
            await target.Handle(testContext, option);
            resourceStore.Verify(r => r.DeleteResources(resourceName, ItMatchesClient(expectedClient)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Delete_Returns403_IfNoClientReadClaim()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{resourceName}")
                .WithPost()
                .WithClaims()
                .WithFile(file, resourceName)
                .TestContext;
            await target.Handle(testContext, option);
            resourceStore.Verify(r => r.UpdateResource(It.IsAny<UpdateResourceRequest>()), Times.Never);
            httpResponseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));
        }

        [Fact]
        public async Task Get_GroupImageSummary_GetsClientSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}")
                .WithClaims()
                .TestContext;
            var expectedResources = new List<ResourceEntryInfo>
            {
                new ResourceEntryInfo{ Name = "File.txt"}
            };
            resourceStore.Setup(r => r.GetResourceCatalogue(ItMatchesClientId(clientGroupImagePath)))
                .ReturnsAsync(() => expectedResources);
            await target.Handle(testContext, option);
            httpResponseFactory.Verify(f => f.BuildJsonResponse(testContext, expectedResources));
        }

        [Fact]
        public async Task Get_GroupImageResource_GetsClientResource()
        {
            var resourceName = "File.txt";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}/{resourceName}")
                .WithClaims()
                .TestContext;
            var expectedResource = new ResourceEntry
            {
                HasEntry = true,
                Content = new MemoryStream(),
                Name = resourceName
            };
            resourceStore.Setup(r => r.GetResource(resourceName, ItMatchesClientId(clientGroupImagePath)))
                .ReturnsAsync(() => expectedResource);
            await target.Handle(testContext, option);
            httpResponseFactory.Verify(f => f.BuildFileResponse(testContext, expectedResource.Content, expectedResource.Name));
        }



        [Fact]
        public async Task Post_GroupImageResource_UploadsResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}/{resourceName}")
                .WithClaims(readClaim)
                .WithPost()
                .WithFile(file, resourceName)
                .TestContext;
            await target.Handle(testContext, option);
            resourceStore.Verify(r => r.UpdateResource(It.Is<UpdateResourceRequest>(a => a.Identity.Client.ClientId.Equals(clientGroupImagePath) && a.Name == resourceName && file == a.Content)));
            httpResponseFactory.Verify(f => f.BuildNoContentResponse(testContext));
        }

        [Fact]
        public async Task Delete_GroupImageResource_DeletesResources()
        {
            var resourceName = "File.txt";
            var file = new MemoryStream();
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientGroupImagePath}/{resourceName}")
                .WithClaims(readClaim)
                .WithDelete()
                .TestContext;
            await target.Handle(testContext, option);
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
                .WithClaims(readClaim)
                .WithJsonBody(new[] { resourceName })
                .TestContext;
            await target.Handle(testContext, option);
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
