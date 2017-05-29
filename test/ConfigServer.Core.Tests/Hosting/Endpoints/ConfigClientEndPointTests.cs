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
    public class ConfigClientEndPointTests
    {
        private readonly Mock<IConfigurationClientService> configurationClientService;
        private readonly Mock<IHttpResponseFactory> factory;
        private readonly Mock<ICommandBus> commandBus;
        private ConfigServerOptions options;
        private static readonly Claim writeClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.WriteClaimValue);
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ReadClaimValue);
        private const string noGroupPath = "None";
        private const string groupClientsPath = "Clients";
        private const string groupId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";

        private IEndpoint target;

        // GET Gets All
        // POST Update Client
        // /{ClientId} GET
        public ConfigClientEndPointTests()
        {
            configurationClientService = new Mock<IConfigurationClientService>();
            factory = new Mock<IHttpResponseFactory>();
            commandBus = new Mock<ICommandBus>();
            target = new ConfigClientEndPoint(configurationClientService.Object, factory.Object, commandBus.Object);
            options = new ConfigServerOptions();
        }

        [Fact]
        public async Task Get_ReturnsAllClients()
        {
            var client = new ConfigurationClient
            {
                ClientId = clientId,
                Name = "Test Client",
                Description = "Description",
                Enviroment = "Dev",
                Group = groupId
            };

            client.Settings.Add("Password", new ConfigurationClientSetting { Key = "Password", Value = "1234" });

            var clients = new List<ConfigurationClient>
            {
                client
            };
            configurationClientService.Setup(cs => cs.GetClients())
                .ReturnsAsync(() => clients);
            
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithClaims(readClaim)
                .TestContext;
            List<ConfigurationClientPayload> observed = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<IEnumerable<ConfigurationClientPayload>>()))
                .Callback((HttpContext c, object arg2) => observed = ((IEnumerable<ConfigurationClientPayload>)arg2).ToList())
                .Returns(() => Task.FromResult(1));

            await target.Handle(context, options);
            Assert.NotNull(observed);
            Assert.Equal(1, observed.Count);
            AssertClient(observed[0], client);
        }

        [Fact]
        public async Task Get_Returns403IfNoReadClaim()
        {
           
            var context = TestHttpContextBuilder.CreateForPath("")
                .WithClaims()
                .TestContext;
            
            await target.Handle(context, options);
            factory.Verify(f => f.BuildStatusResponse(context, 403));
        }

        [Fact]
        public async Task Get_ClientId_ReturnsClient_IfFound()
        {
            var client = new ConfigurationClient
            {
                ClientId = clientId,
                Name = "Test Client",
                Description = "Description",
                Enviroment = "Dev",
                Group = groupId
            };

            client.Settings.Add("Password", new ConfigurationClientSetting { Key = "Password", Value = "1234" });

            configurationClientService.Setup(cs => cs.GetClientOrDefault(clientId))
                .ReturnsAsync(() => client);

            var context = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(readClaim).TestContext;
            ConfigurationClientPayload observed = null;
            factory.Setup(f => f.BuildJsonResponse(context, It.IsAny<ConfigurationClientPayload>()))
                .Callback((HttpContext c, object arg2) => observed = (ConfigurationClientPayload)arg2)
                .Returns(() => Task.FromResult(1));

            await target.Handle(context, options);
            Assert.NotNull(observed);
            AssertClient(observed, client);
        }

        [Fact]
        public async Task Get_ClientId_ReturnsNotFound_IfNotFound()
        {
            

            configurationClientService.Setup(cs => cs.GetClientOrDefault(clientId))
                .ReturnsAsync(() => null);

            var context = TestHttpContextBuilder.CreateForPath($"/{clientId}")
                .WithClaims(readClaim).TestContext;
            await target.Handle(context, options);
            factory.Verify(f => f.BuildNotFoundStatusResponse(context));
        }

        [Fact]
        public async Task Post_CallsCommandBus()
        {
            var client = new ConfigurationClientPayload
            {
                ClientId = clientId,
            };

            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithClaims(writeClaim)
                .WithPost()
                .WithJsonBody(client)
                .TestContext;

            await target.Handle(context, options);
            commandBus.Verify(f => f.SubmitAsync(It.Is<CreateUpdateClientCommand>(c => c.Client.ClientId == client.ClientId)));

        }

        [Fact]
        public async Task Post_BuildsResponseFromCommandResult()
        {
            var client = new ConfigurationClientPayload
            {
                ClientId = clientId,
            };

            var commandResult = CommandResult.Success();
            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithClaims(writeClaim)
                .WithPost()
                .WithJsonBody(client)
                .TestContext;
            commandBus.Setup(c => c.SubmitAsync(It.IsAny<CreateUpdateClientCommand>()))
                .ReturnsAsync(() => commandResult);

            await target.Handle(context, options);
            factory.Verify(f => f.BuildResponseFromCommandResult(context, commandResult));

        }

        [Fact]
        public async Task Post_403_IfDoesNotHaveWriteClaim()
        {
            var client = new ConfigurationClientPayload
            {
                ClientId = clientId,
            };

            var commandResult = CommandResult.Success();
            var context = TestHttpContextBuilder.CreateForPath("/")
                .WithClaims(readClaim)
                .WithPost()
                .WithJsonBody(client)
                .TestContext;
            

            await target.Handle(context, options);
            commandBus.Verify(c => c.SubmitAsync(It.IsAny<CreateUpdateClientCommand>()), Times.Never);
            factory.Verify(f => f.BuildStatusResponse(context, 403));

        }

        private void AssertClient(ConfigurationClientPayload payload, ConfigurationClient client)
        {
            Assert.Equal(payload.ClientId, client.ClientId);
            Assert.Equal(payload.Name, client.Name);
            Assert.Equal(payload.Description, client.Description);
            Assert.Equal(payload.Enviroment, client.Enviroment);
            Assert.Equal(payload.Group, client.Group);
            Assert.Equal(payload.Settings.Select(s => s.Key), client.Settings.Values.Select(s => s.Key));
            Assert.Equal(payload.Settings.Select(s => s.Value), client.Settings.Values.Select(s => s.Value));

        }
    }
}
