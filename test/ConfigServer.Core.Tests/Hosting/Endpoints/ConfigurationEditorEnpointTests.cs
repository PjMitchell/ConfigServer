using ConfigServer.Server;
using ConfigServer.TestModels;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ConfigurationEditorEnpointTests
    {
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly Mock<IConfigurationSetModelPayloadMapper> modelPayloadMapper;
        private readonly Mock<IConfigInstanceRouter> configInstanceRouter;
        private readonly Mock<IConfigurationEditModelMapper> configurationEditModelMapper;
        private readonly ConfigurationModelRegistry configCollection;
        private readonly Mock<IConfigurationClientService> configClientService;
        private readonly Mock<ICommandBus> commandBus;
        private const string valuePath = "Value";

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private ConfigServerOptions options;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;

        // /{ Client Id}/{ config name}
        // GET: Gets Config model for editor
        // POST: Sets Config from editor model
        public ConfigurationEditorEnpointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            responseFactory = new Mock<IHttpResponseFactory>();
            modelPayloadMapper = new Mock<IConfigurationSetModelPayloadMapper>();
            configInstanceRouter = new Mock<IConfigInstanceRouter>();
            configurationEditModelMapper = new Mock<IConfigurationEditModelMapper>();
            configCollection = new ConfigurationModelRegistry();
            var configSetModel = new ConfigurationSetModel<SampleConfigSet>("Sample", "Sample description");
            configSetModel.GetOrInitialize(set => set.SampleConfig);
            configCollection.AddConfigurationSet(configSetModel);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            commandBus = new Mock<ICommandBus>();

            target = new ConfigurationEditorEndpoint(responseFactory.Object, configInstanceRouter.Object, configurationEditModelMapper.Object, configCollection, configClientService.Object, commandBus.Object);
            options = new ConfigServerOptions();
        }

        [Fact]
        public async Task Get_ReturnsConfigurationEditModelForClient()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{typeof(SampleConfig).Name}").WithClaims(readClaim).TestContext;
            var configInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), new ConfigurationIdentity(expectedClient, new Version(1, 0)));
            configInstanceRouter.Setup(r => r.GetConfigInstanceOrDefault(expectedClient, typeof(SampleConfig).Name))
                .ReturnsAsync(() => configInstance);
            var mappedModel = new object();

            configurationEditModelMapper.Setup(m => m.MapToEditConfig(configInstance,(configCollection.GetConfigDefinition(typeof(SampleConfig)))))
                .Returns(() => mappedModel);

            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildJsonResponse(testContext, mappedModel));

        }

        [Fact]
        public async Task Get_Returns403IfNoConfiguratorClaimOnClient()
        {
            expectedClient.ConfiguratorClaim = "Denied";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{typeof(SampleConfig).Name}").WithClaims(readClaim).TestContext;
            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildStatusResponse(testContext, 403));

        }

        [Fact]
        public async Task Post_ReturnsCommandResultFromBus()
        {
            var stringValue = "Hello";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{typeof(SampleConfig).Name}")
                .WithClaims(readClaim)
                .WithPost()
                .WithStringBody(stringValue)
                .TestContext;
            var commandResult = CommandResult.Success();
            commandBus.Setup(c => c.SubmitAsync(It.Is<UpdateConfigurationFromEditorCommand>(command => command.Identity.Client.Equals(expectedClient) && command.ConfigurationType == typeof(SampleConfig) && command.ConfigurationAsJson == stringValue)))
                .ReturnsAsync(() => commandResult);

            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildResponseFromCommandResult(testContext, commandResult));

        }

        [Fact]
        public async Task Post_Returns403IfNoConfiguratorClaimOnClient()
        {
            expectedClient.ConfiguratorClaim = "Denied";
            var stringValue = "Hello";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{typeof(SampleConfig).Name}")
                .WithClaims(readClaim)
                .WithPost()
                .WithStringBody(stringValue)
                .TestContext;
            await target.Handle(testContext, options);
            commandBus.Verify(c => c.SubmitAsync(It.IsAny<UpdateConfigurationFromEditorCommand>()),Times.Never);


            responseFactory.Verify(v => v.BuildStatusResponse(testContext, 403));

        }
    }
}
