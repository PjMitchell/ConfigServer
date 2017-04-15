using ConfigServer.Core.Hosting;
using ConfigServer.Server;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ClientGroupEndpointTests
    {
        private readonly Mock<IConfigurationClientService> configurationClientService;
        private readonly Mock<IHttpResponseFactory> factory;
        private readonly Mock<ICommandBus> commandBus;

        private const string noGroupPath = "None";
        private const string groupClientsPath = "Clients";
        private const string groupId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private const string groupId2 = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";

        private IEndpoint target;

        // GET Gets All Groups
        // POST Update Groups
        // /{GroupId} GET
        // /{GroupId}/Clients GET
        // /None/Clients GET
        public ClientGroupEndpointTests()
        {
            configurationClientService = new Mock<IConfigurationClientService>();
            factory = new Mock<IHttpResponseFactory>();
            commandBus = new Mock<ICommandBus>();
            target = new ClientGroupEndpoint(configurationClientService.Object, factory.Object, commandBus.Object);
        }

        [Fact]
        public async Task Get_GetsAllGroups()
        {
            var groups = new List<ConfigurationClientGroup>
            {
                new ConfigurationClientGroup{ GroupId = groupId}
            };
            configurationClientService.Setup(s => s.GetGroups())
                .ReturnsAsync(() => groups);
            var context = TestHttpContextBuilder.CreateForPath("/").TestContext;

            var result = await target.TryHandle(context);
            Assert.True(result);
            factory.Verify(f => f.BuildJsonResponse(context, groups));

        }

        [Fact]
        public async Task Get_Group_GetsGroup()
        {
            var group = new ConfigurationClientGroup { GroupId = groupId };

            configurationClientService.Setup(s => s.GetClientGroupOrDefault(groupId))
                .ReturnsAsync(() => group);
            var context = TestHttpContextBuilder.CreateForPath($"/{groupId}").TestContext;

            var result = await target.TryHandle(context);
            Assert.True(result);
            factory.Verify(f => f.BuildJsonResponse(context, group));
        }

        [Fact]
        public async Task Get_Group_ReturnsNotFoundIfGroupNotFound()
        {
            var group = new ConfigurationClientGroup { GroupId = groupId };

            configurationClientService.Setup(s => s.GetClientGroupOrDefault(groupId))
                .ReturnsAsync(() => null);
            var context = TestHttpContextBuilder.CreateForPath($"/{groupId}").TestContext;

            var result = await target.TryHandle(context);
            Assert.True(result);
            factory.Verify(f => f.BuildNotFoundStatusResponse(context));
        }

        [Fact]
        public async Task Get_Group_Client_ReturnsClientsForGroup()
        {
            var clients = new List<ConfigurationClient>
            {
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId, Name = "Name1"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId2, Name = "Name2"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId, Name = "Name3"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Name = "Name4"}
            };

            configurationClientService.Setup(s => s.GetClients())
                .ReturnsAsync(() => clients);
            var context = TestHttpContextBuilder.CreateForPath($"/{groupId}/{groupClientsPath}").TestContext;
            var observed = new List<ConfigurationClient>();
            factory.Setup(f => f.BuildJsonResponse(context,It.IsAny<IEnumerable<ConfigurationClient>>()))
                .Callback((HttpContext c, object arg2)=> observed = ((IEnumerable<ConfigurationClient>)arg2).ToList())
                .Returns(()=> Task.FromResult(1));
            var result = await target.TryHandle(context);
            Assert.True(result);
            Assert.Equal(clients.Where(w => groupId.Equals(w.Group)).ToList(), observed);
        }

        [Fact]
        public async Task Get_NoGroup_Client_ReturnsClientsWithoutGroup()
        {
            var clients = new List<ConfigurationClient>
            {
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId, Name = "Name1"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId2, Name = "Name2"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId, Name = "Name3"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Name = "Name4"}
            };
            var groups = new List<ConfigurationClientGroup>
            {
                new ConfigurationClientGroup{ GroupId = groupId},
                new ConfigurationClientGroup{ GroupId = groupId2}
            };
            configurationClientService.Setup(s => s.GetGroups())
                .ReturnsAsync(() => groups);
            configurationClientService.Setup(s => s.GetClients())
                .ReturnsAsync(() => clients);
            var context = TestHttpContextBuilder.CreateForPath($"/{noGroupPath}/{groupClientsPath}").TestContext;
            var observed = new List<ConfigurationClient>();
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<IEnumerable<ConfigurationClient>>()))
                .Callback((HttpContext c, object arg2) => observed = ((IEnumerable<ConfigurationClient>)arg2).ToList())
                .Returns(() => Task.FromResult(1));
            var result = await target.TryHandle(context);
            Assert.True(result);
            Assert.Equal(clients.Where(w => String.IsNullOrWhiteSpace(w.Group)).ToList(), observed);
        }

        [Fact]
        public async Task Get_NoGroup_Client_ReturnsClientsWithUnknownGroup()
        {
            var clients = new List<ConfigurationClient>
            {
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId, Name = "Name1"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId2, Name = "Name2"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Group = groupId, Name = "Name3"},
                new ConfigurationClient{ ClientId = Guid.NewGuid().ToString(), Name = "Name4"}
            };
            var groups = new List<ConfigurationClientGroup>
            {
                new ConfigurationClientGroup{ GroupId = groupId}
            };
            configurationClientService.Setup(s => s.GetGroups())
                .ReturnsAsync(() => groups);
            configurationClientService.Setup(s => s.GetClients())
                .ReturnsAsync(() => clients);
            var context = TestHttpContextBuilder.CreateForPath($"/{noGroupPath}/{groupClientsPath}").TestContext;
            var observed = new List<ConfigurationClient>();
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<IEnumerable<ConfigurationClient>>()))
                .Callback((HttpContext c, object arg2) => observed = ((IEnumerable<ConfigurationClient>)arg2).ToList())
                .Returns(() => Task.FromResult(1));
            var result = await target.TryHandle(context);
            Assert.True(result);
            Assert.Equal(clients.Where(w => !groupId.Equals(w.Group)).ToList(), observed);
        }

        [Fact]
        public async Task Post_CallsCommandBus()
        {
            var group = new ConfigurationClientGroup{ GroupId = groupId};

            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithPost()
                .WithJsonBody(group)
                .TestContext;

            var result = await target.TryHandle(context);
            Assert.True(result);
            commandBus.Verify(f => f.SubmitAsync(It.Is<CreateUpdateClientGroupCommand>(c=> c.ClientGroup.GroupId == group.GroupId)));

        }

        [Fact]
        public async Task Post_BuildsResponseFromCommandResult()
        {
            var group = new ConfigurationClientGroup { GroupId = groupId };
            var commandResult = CommandResult.Success();
            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithPost()
                .WithJsonBody(group)
                .TestContext;
            commandBus.Setup(f => f.SubmitAsync(It.IsAny<CreateUpdateClientGroupCommand>()))
                .ReturnsAsync(()=> commandResult);

            var result = await target.TryHandle(context);
            Assert.True(result);
            factory.Verify(f => f.BuildResponseFromCommandResult(context, commandResult));

        }

        [Fact]
        public async Task Post_ReturnsInvalidRequest_IfNoBody()
        {
            var group = new ConfigurationClientGroup { GroupId = groupId };

            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithPost()
                .TestContext;

            var result = await target.TryHandle(context);
            Assert.True(result);
            factory.Verify(f => f.BuildInvalidRequestResponse(context, It.IsAny<IEnumerable<string>>()));
        }
    }
}
