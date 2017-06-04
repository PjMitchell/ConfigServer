using ConfigServer.Sample.Models;
using ConfigServer.Server;
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
    public class UploadEnpointTests
    {
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly ConfigurationSetRegistry configCollection;
        private readonly Mock<ICommandBus> commandBus;
        private readonly Mock<IConfigurationClientService> configClientService;
        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private const string notFoundClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D2";
        private ConfigServerOptions option;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;

        // /ConfigurationSet/{clientId}/{Configuration Set}
        // POST: Uploads configuration set file
        // /Configuration/{clientId}/{Config name}
        // POST: Uploads configuration file
        public UploadEnpointTests()
        {
            responseFactory = new Mock<IHttpResponseFactory>();
            var testConfigSet = new SampleConfigSet();
            configCollection = new ConfigurationSetRegistry();
            configCollection.AddConfigurationSet(testConfigSet.BuildConfigurationSetModel());
            commandBus = new Mock<ICommandBus>();
            expectedClient = new ConfigurationClient(clientId);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            option = new ConfigServerOptions();
            target = new UploadEnpoint(responseFactory.Object, configCollection, commandBus.Object, configClientService.Object);
        }

        [Fact]
        public async Task Upload_ConfigSet_RaiseUploadSetCommand()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/ConfigurationSet/{clientId}/{nameof(SampleConfigSet)}")
                .WithClaims(readClaim)
                .WithPost()
                .WithStringBody(json)
                .TestContext;
            var commandResult = CommandResult.Success();
            commandBus.Setup(cb => cb.SubmitAsync(It.Is<UpdateConfigurationSetFromJsonUploadCommand>(c => c.ConfigurationSetType == typeof(SampleConfigSet) && c.JsonUpload == json && c.Identity.Client.Equals(expectedClient))))
                .ReturnsAsync(() => commandResult);
            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildResponseFromCommandResult(testContext, commandResult));

        }

        [Fact]
        public async Task Upload_Returns403_IfNoClaim()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/ConfigurationSet/{clientId}/{nameof(SampleConfigSet)}")
                .WithClaims()
                .WithPost()
                .WithStringBody(json)
                .TestContext;
            var commandResult = CommandResult.Success();
            
            await target.Handle(testContext, option);
            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<UpdateConfigurationSetFromJsonUploadCommand>()), Times.Never);
            responseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));

        }

        [Fact]
        public async Task Upload_ConfigSet_NotFoundIfClientNotFound()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/ConfigurationSet/{notFoundClientId}/{nameof(SampleConfigSet)}")
                .WithClaims(readClaim)
                .WithPost()
                .WithStringBody(json)
                .TestContext;


            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<UpdateConfigurationSetFromJsonUploadCommand>()), Times.Never);

        }

        [Fact]
        public async Task Upload_Config_RaiseUploadSetCommand()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/Configuration/{clientId}/{nameof(SampleConfig)}")
                .WithClaims(readClaim)
                .WithPost()
                .WithStringBody(json)
                .TestContext;
            var commandResult = CommandResult.Success();
            commandBus.Setup(cb => cb.SubmitAsync(It.Is<UpdateConfigurationFromJsonUploadCommand>(c => c.ConfigurationType == typeof(SampleConfig) && c.JsonUpload == json && c.Identity.Client.Equals(expectedClient))))
                .ReturnsAsync(() => commandResult);
            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildResponseFromCommandResult(testContext, commandResult));

        }

        [Fact]
        public async Task Upload_Config_NotFoundIfClientNotFound()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/Configuration/{notFoundClientId}/{nameof(SampleConfig)}")
                .WithClaims(readClaim)
                .WithPost()
                .WithStringBody(json)
                .TestContext;


            await target.Handle(testContext, option);
            responseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<UpdateConfigurationFromJsonUploadCommand>()), Times.Never);

        }
    }
}
