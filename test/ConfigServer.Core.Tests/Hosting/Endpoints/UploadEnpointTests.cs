using ConfigServer.Sample.Models;
using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class UploadEnpointTests
    {
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly IConfigurationSetRegistry configCollection;
        private readonly Mock<ICommandBus> commandBus;
        private readonly Mock<IConfigurationClientService> configClientService;
        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private const string notFoundClientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D2";

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
            target = new UploadEnpoint(responseFactory.Object, configCollection, commandBus.Object, configClientService.Object);
        }

        [Fact]
        public async Task Upload_ConfigSet_RaiseUploadSetCommand()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/ConfigurationSet/{clientId}/{nameof(SampleConfigSet)}")
                .WithPost()
                .WithStringBody(json)
                .TestContext;
            var commandResult = CommandResult.Success();
            commandBus.Setup(cb => cb.SubmitAsync(It.Is<UpdateConfigurationSetFromJsonUploadCommand>(c => c.ConfigurationSetType == typeof(SampleConfigSet) && c.JsonUpload == json && c.Identity.Client.Equals(expectedClient))))
                .ReturnsAsync(() => commandResult);
            var result = await target.TryHandle(testContext);
            Assert.True(result);
            responseFactory.Verify(f => f.BuildResponseFromCommandResult(testContext, commandResult));

        }

        [Fact]
        public async Task Upload_ConfigSet_NotFoundIfClientNotFound()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/ConfigurationSet/{notFoundClientId}/{nameof(SampleConfigSet)}")
                .WithPost()
                .WithStringBody(json)
                .TestContext;


            var result = await target.TryHandle(testContext);
            Assert.True(result);
            responseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<UpdateConfigurationSetFromJsonUploadCommand>()), Times.Never);

        }

        [Fact]
        public async Task Upload_Config_RaiseUploadSetCommand()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/Configuration/{clientId}/{nameof(SampleConfig)}")
                .WithPost()
                .WithStringBody(json)
                .TestContext;
            var commandResult = CommandResult.Success();
            commandBus.Setup(cb => cb.SubmitAsync(It.Is<UpdateConfigurationFromJsonUploadCommand>(c => c.ConfigurationType == typeof(SampleConfig) && c.JsonUpload == json && c.Identity.Client.Equals(expectedClient))))
                .ReturnsAsync(() => commandResult);
            var result = await target.TryHandle(testContext);
            Assert.True(result);
            responseFactory.Verify(f => f.BuildResponseFromCommandResult(testContext, commandResult));

        }

        [Fact]
        public async Task Upload_Config_NotFoundIfClientNotFound()
        {
            var json = "{}";
            var testContext = TestHttpContextBuilder.CreateForPath($"/Configuration/{notFoundClientId}/{nameof(SampleConfig)}")
                .WithPost()
                .WithStringBody(json)
                .TestContext;


            var result = await target.TryHandle(testContext);
            Assert.True(result);
            responseFactory.Verify(f => f.BuildNotFoundStatusResponse(testContext));
            commandBus.Verify(cb => cb.SubmitAsync(It.IsAny<UpdateConfigurationFromJsonUploadCommand>()), Times.Never);

        }
    }
}
