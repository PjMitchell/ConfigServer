using ConfigServer.Server;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class PermisionEndpointTest
    {
        private readonly IEndpoint target;
        private readonly Mock<IHttpResponseFactory> factory;

        private ConfigServerOptions options;
        private static readonly Claim writeClientAdminClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.WriteClaimValue);
        private static readonly Claim readClientAdminClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ReadClaimValue);
        public PermisionEndpointTest()
        {
            options = new ConfigServerOptions();
            factory = new Mock<IHttpResponseFactory>();
            target = new PermissionEndpoint(factory.Object);
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
            Assert.False(permission.CanAddClients);
            Assert.False(permission.CanAddGroups);

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
            Assert.False(permission.CanAddClients);
            Assert.False(permission.CanAddGroups);

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
            Assert.True(permission.CanAddClients);
            Assert.True(permission.CanAddGroups);

        }

    }
}
