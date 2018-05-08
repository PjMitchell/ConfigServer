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
    public class SnapshotEndpointTests
    {
        readonly Mock<IHttpResponseFactory> httpResponseFactory;
        readonly Mock<IConfigurationSnapshotRepository> snapShotRepository;
        readonly Mock<ICommandBus> commandBus;
        readonly Mock<IConfigurationClientService> configurationClientService;
        readonly ConfigServerOptions options;
        readonly ConfigurationClient client;
        readonly IEndpoint target;
        private const string groupId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private const string clientId = "9E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private const string groupId2 = "3E37AC18-A00F-47A5-B84E-C29E0823F6D4";
        private static readonly Claim configuratorClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);

        public SnapshotEndpointTests()
        {
            httpResponseFactory = new Mock<IHttpResponseFactory>();
            snapShotRepository = new Mock<IConfigurationSnapshotRepository>();
            client = new ConfigurationClient { ClientId = clientId };
            commandBus = new Mock<ICommandBus>();
            configurationClientService = new Mock<IConfigurationClientService>();
            configurationClientService.Setup(c => c.GetClientOrDefault(clientId))
                .ReturnsAsync(client);

            options = new ConfigServerOptions();
            target = new SnapshotEndpoint(snapShotRepository.Object, commandBus.Object, configurationClientService.Object, httpResponseFactory.Object);
        }

        [Fact]
        public async Task Get_Group_GetsAllSnapshotsForGroup()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/group/{groupId}")
                .WithClaims(configuratorClaim)
                .TestContext;
            var snapshots = new SnapshotEntryInfo[]
            {
                new SnapshotEntryInfo{ Id = Guid.NewGuid().ToString(), GroupId = groupId, Name = "This One", TimeStamp = new DateTime(2013,10,1)},
                new SnapshotEntryInfo{ Id = Guid.NewGuid().ToString(), GroupId = groupId2, Name = "Not This One", TimeStamp = new DateTime(2013,10,2)}
            };
            snapShotRepository.Setup(s => s.GetSnapshots())
                .ReturnsAsync(snapshots);
            var observed = new SnapshotEntryInfo[0];
            httpResponseFactory.Setup(f => f.BuildJsonResponse(context, It.IsAny<IEnumerable<SnapshotEntryInfo>>()))
                .Returns((HttpContext c, object config) =>
                {
                    observed = (config as IEnumerable<SnapshotEntryInfo>)?.ToArray() ?? new SnapshotEntryInfo[0];
                    return Task.FromResult(true);
                });
            await target.Handle(context, options);

            Assert.Single(observed);
            Assert.Equal(snapshots[0].Id, observed[0].Id);
            Assert.Equal(groupId, observed[0].GroupId);
            Assert.Equal(snapshots[0].Name, observed[0].Name);
            Assert.Equal(snapshots[0].TimeStamp, observed[0].TimeStamp);
        }

        [Fact]
        public async Task Get_Group_ReturnsForbiddenIFNoConfiguratorClaim()
        {
            var context = TestHttpContextBuilder.CreateForPath($"/group/{groupId}")
                .WithClaims()
                .TestContext;           
            
            await target.Handle(context, options);

            httpResponseFactory.Verify(f => f.BuildStatusResponse(context, 403));
        }

        [Fact]
        public async Task Post_RaisesCommand()
        {
            var command = new CreateSnapshotCommand { Name = "1.0.0", ClientId = "3E37AC18-A00F-47A5-B84E-C29E0823F6D8" };
            var context = TestHttpContextBuilder.CreateForPath($"/")
                .WithPost()
                .WithJsonBody<CreateSnapshotCommand>(command)
                .WithClaims(configuratorClaim)
                .TestContext;
            var commandResult = CommandResult.Success();

            commandBus.Setup(cb => cb.SubmitAsync<CreateSnapshotCommand>(It.Is<CreateSnapshotCommand>(s=> s.Name == command.Name && s.ClientId == command.ClientId)))
                .ReturnsAsync(commandResult);

            await target.Handle(context, options);

            httpResponseFactory.Verify(f => f.BuildResponseFromCommandResult(context, commandResult));

            
        }

        [Fact]
        public async Task Post_ReturnsForbiddenIFNoConfiguratorClaim()
        {
            var command = new CreateSnapshotCommand { Name = "1.0.0", ClientId= "3E37AC18-A00F-47A5-B84E-C29E0823F6D8" };
            var context = TestHttpContextBuilder.CreateForPath($"/")
                .WithPost()
                .WithJsonBody<CreateSnapshotCommand>(command)
                .WithClaims()
                .TestContext;
            var commandResult = CommandResult.Success();
            await target.Handle(context, options);

            commandBus.Verify(cb => cb.SubmitAsync<CreateSnapshotCommand>(It.IsAny<CreateSnapshotCommand>()), Times.Never);

            httpResponseFactory.Verify(f => f.BuildStatusResponse(context, 403));
        }

        [Fact]
        public async Task Delete_RaisesCommand()
        {
            var id = "3E37AC18-A00F-47A5-B84E-C29E0823F6D8";

            var context = TestHttpContextBuilder.CreateForPath($"/{id}")
                .WithDelete()
                .WithClaims(configuratorClaim)
                .TestContext;
            var commandResult = CommandResult.Success();

            commandBus.Setup(cb => cb.SubmitAsync<DeleteSnapshotCommand>(It.Is<DeleteSnapshotCommand>(s => s.SnapshotId == id)))
                .ReturnsAsync(commandResult);

            await target.Handle(context, options);

            httpResponseFactory.Verify(f => f.BuildResponseFromCommandResult(context, commandResult));


        }

        [Fact]
        public async Task Delete_ReturnsForbiddenIFNoConfiguratorClaim()
        {
            var id = "3E37AC18-A00F-47A5-B84E-C29E0823F6D8";
            
            var context = TestHttpContextBuilder.CreateForPath($"/{id}")
                .WithDelete()
                .WithClaims()
                .TestContext;
            var commandResult = CommandResult.Success();
            await target.Handle(context, options);

            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<DeleteSnapshotCommand>()), Times.Never);

            httpResponseFactory.Verify(f => f.BuildStatusResponse(context, 403));
        }

        [Fact]
        public async Task Push_RaisesCommand()
        {
            var id = "3E37AC18-A00F-47A5-B84E-C29E0823F6D8";
            var configToCopy = "SimpleConfig";
            var context = TestHttpContextBuilder.CreateForPath($"/{id}/to/{clientId}")
                .WithPost()
                .WithJsonBody(new PushSnapshotToClientRequest { ConfigsToCopy = new []{ configToCopy } })
                .WithClaims(configuratorClaim)
                .TestContext;
            var commandResult = CommandResult.Success();

            commandBus.Setup(cb => cb.SubmitAsync(It.Is<PushSnapshotToClientCommand>(s => s.SnapshotId == id && s.TargetClient.Equals(client) && s.ConfigsToCopy.Length == 1 && s.ConfigsToCopy[0] == configToCopy )))
                .ReturnsAsync(commandResult);

            await target.Handle(context, options);

            httpResponseFactory.Verify(f => f.BuildResponseFromCommandResult(context, commandResult));


        }

        [Fact]
        public async Task Push_ReturnsForbiddenIFNoConfiguratorClaim()
        {
            var id = "3E37AC18-A00F-47A5-B84E-C29E0823F6D8";

            var context = TestHttpContextBuilder.CreateForPath($"/{id}/to/{clientId}")
                .WithPost()
                .WithJsonBody(new PushSnapshotToClientRequest())
                .WithClaims()
                .TestContext;
            var commandResult = CommandResult.Success();
            await target.Handle(context, options);

            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<PushSnapshotToClientCommand>()), Times.Never);

            httpResponseFactory.Verify(f => f.BuildStatusResponse(context, 403));
        }

        [Fact]
        public async Task Push_ReturnsForbiddenIFNoClientClaim()
        {
            var id = "3E37AC18-A00F-47A5-B84E-C29E0823F6D8";
            client.ConfiguratorClaim = "ClaimYouDoNotHave";
            var context = TestHttpContextBuilder.CreateForPath($"/{id}/to/{clientId}")
                .WithPost()
                .WithJsonBody(new PushSnapshotToClientRequest { ConfigsToCopy = new[] { "SimpleCOnfig" } })
                .WithClaims(configuratorClaim)
                .TestContext;
            var commandResult = CommandResult.Success();
            await target.Handle(context, options);

            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<PushSnapshotToClientCommand>()), Times.Never);

            httpResponseFactory.Verify(f => f.BuildStatusResponse(context, 403));
        }

        // / POST Save snapshot
        // /{snapShotId}
        // DELETE: Deletes snapShot

        // /Group/{clientGroupId}
        //  GET: Gets SnapshotIds for clientGroupId

        //Required Claim: option.ClientAdminClaimType
        //GET DELETE 'configurator', 'admin'

        // /{ snapShotId}/to/{clientId}
    }
}
