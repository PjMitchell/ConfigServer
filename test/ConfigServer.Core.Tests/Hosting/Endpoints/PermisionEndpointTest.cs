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
    public class PermisionEndpointTest
    {
        private readonly IEndpoint target;
        private readonly Mock<IHttpResponseFactory> factory;
        private readonly Mock<IConfigurationClientService> clientService;

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private ConfigServerOptions options;
        private static readonly Claim writeClientAdminClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.AdminClaimValue);
        private static readonly Claim readClientAdminClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        public PermisionEndpointTest()
        {
            options = new ConfigServerOptions();
            factory = new Mock<IHttpResponseFactory>();
            clientService = new Mock<IConfigurationClientService>();
            clientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(expectedClient);
            target = new PermissionEndpoint(factory.Object, clientService.Object);
            expectedClient = new ConfigurationClient { ClientId = clientId, ConfiguratorClaim = "Configurator" };
        }

        [Fact]
        public async Task ReturnsMethodNotAcceptedIfNotGet()
        {
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithPost()
                .TestContext;
            await target.Handle(context, options);
            factory.Verify(f => f.BuildMethodNotAcceptedStatusResponse(context));
        }

        [Fact]
        public async Task CorrectlyMapsNoClaims()
        {
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithClaims()
                .TestContext;
            UserPermissions permission = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<UserPermissions>()))
                .Returns((HttpContext c, UserPermissions p) => {
                    permission = p;
                    return Task.FromResult(true);
                });

            await target.Handle(context, options);
            Assert.NotNull(permission);
            Assert.False(permission.CanAccessClientAdmin);
            Assert.False(permission.CanEditClients);
            Assert.False(permission.CanEditGroups);
            Assert.False(permission.CanDeleteArchives);

            

        }

        [Fact]
        public async Task CorrectlyMapsReadAdminClaim()
        {
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithClaims(readClientAdminClaim)
                .TestContext;
            UserPermissions permission = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<UserPermissions>()))
                .Returns((HttpContext c, UserPermissions p) => {
                    permission = p;
                    return Task.FromResult(true);
                });

            await target.Handle(context, options);
            Assert.NotNull(permission);
            Assert.True(permission.CanAccessClientAdmin);
            Assert.False(permission.CanEditClients);
            Assert.False(permission.CanEditGroups);
            Assert.False(permission.CanDeleteArchives);

        }

        [Fact]
        public async Task CorrectlyMapsWriteAdminClaim()
        {
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithClaims(writeClientAdminClaim)
                .TestContext;
            UserPermissions permission = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<UserPermissions>()))
                .Returns((HttpContext c, UserPermissions p) => {
                    permission = p;
                    return Task.FromResult(true);
                });

            await target.Handle(context, options);
            Assert.NotNull(permission);
            Assert.True(permission.CanAccessClientAdmin);
            Assert.True(permission.CanEditClients);
            Assert.True(permission.CanEditGroups);
            Assert.True(permission.CanDeleteArchives);

        }


        [Fact]
        public async Task CorrectlyMapsClientConfiguratorClaims()
        {
            var expected = new List<string> { "Test", "Test3" };
            var claims = expected.Select(s => new Claim(options.ClientConfiguratorClaimType, s)).ToArray();
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithClaims(claims)
                .TestContext;
            UserPermissions permission = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<UserPermissions>()))
                .Returns((HttpContext c, UserPermissions p) => {
                    permission = p;
                    return Task.FromResult(true);
                });

            await target.Handle(context, options);
            Assert.NotNull(permission);
            Assert.Equal(expected, permission.ClientConfiguratorClaims);

        }

        [Fact]
        public async Task CorrectlyMapsWriteAdminClaimForClient()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(writeClientAdminClaim)
                .TestContext;
            UserClientPermissions permission = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<UserClientPermissions>()))
                .Returns((HttpContext c, UserClientPermissions p) => {
                    permission = p;
                    return Task.FromResult(true);
                });

            await target.Handle(context, options);
            Assert.NotNull(permission);
            Assert.True(permission.CanAccessClientAdmin);
            Assert.True(permission.CanEditClients);
            Assert.True(permission.CanEditGroups);
            Assert.True(permission.CanDeleteArchives);

        }

        [Fact]
        public async Task CorrectlyMapsClientClaim()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(writeClientAdminClaim)
                .TestContext;
            UserClientPermissions permission = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<UserClientPermissions>()))
                .Returns((HttpContext c, UserClientPermissions p) => {
                    permission = p;
                    return Task.FromResult(true);
                });

            await target.Handle(context, options);
            Assert.NotNull(permission);
            Assert.False(permission.HasClientConfiguratorClaim);
        }

    }
}
