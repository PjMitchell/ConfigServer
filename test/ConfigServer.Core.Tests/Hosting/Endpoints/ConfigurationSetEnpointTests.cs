using ConfigServer.Sample.Models;
using ConfigServer.Server;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ConfigurationSetEnpointTests
    {
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly Mock<IConfigurationSetModelPayloadMapper> modelPayloadMapper;
        private readonly Mock<IConfigInstanceRouter> configInstanceRouter;
        private readonly Mock<IConfigurationEditModelMapper> configurationEditModelMapper;
        private readonly ConfigurationSetRegistry configCollection;
        private readonly Mock<IConfigurationClientService> configClientService;
        private readonly Mock<ICommandBus> commandBus;
        private const string modelPath = "Model";
        private const string valuePath = "Value";

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private ConfigServerOptions options;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;

        // GET: Gets all configuration set summaries
        // Model/{ Client Id}/{ Configuration Set}
        // GET: Model for configuration set
        // Value/{ Client Id}/{ config name}
        // GET: Gets Config model for editor
        // POST: Sets Config from editor model
        public ConfigurationSetEnpointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            responseFactory = new Mock<IHttpResponseFactory>();
            modelPayloadMapper = new Mock<IConfigurationSetModelPayloadMapper>();
            configInstanceRouter = new Mock<IConfigInstanceRouter>();
            configurationEditModelMapper = new Mock<IConfigurationEditModelMapper>();
            configCollection = new ConfigurationSetRegistry();
            var configSetModel = new ConfigurationSetModel<SampleConfigSet>("Sample", "Sample description");
            configSetModel.GetOrInitialize(set => set.SampleConfig);
            configCollection.AddConfigurationSet(configSetModel);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(() => expectedClient);
            commandBus = new Mock<ICommandBus>();

            target = new ConfigurationSetEnpoint(responseFactory.Object, modelPayloadMapper.Object, configInstanceRouter.Object, configurationEditModelMapper.Object, configCollection, configClientService.Object, commandBus.Object);
            options = new ConfigServerOptions();
        }

        [Fact]
        public async Task Get_ReturnsConfigurationSetSummary()
        {
            var testContext = TestHttpContextBuilder.CreateForPath("/")
                .WithClaims(readClaim)
                .TestContext;
            List<ConfigurationSetSummary> summaries = null;
            responseFactory.Setup(f => f.BuildJsonResponse(testContext, It.IsAny<IEnumerable<ConfigurationSetSummary>>()))
                .Callback((HttpContext c,object arg) => summaries =((IEnumerable<ConfigurationSetSummary>)arg).ToList())
                .Returns(()=> Task.FromResult(true));
            await target.Handle(testContext, options);
            Assert.Equal(1, summaries.Count);
            var summary = summaries.Single();
            var model = configCollection.Single();
            Assert.Equal(model.ConfigSetType.Name, summary.ConfigurationSetId);
            Assert.Equal(model.Name, summary.Name);
            Assert.Equal(model.Description, summary.Description);
            Assert.Equal(1, summary.Configs.Count);
            var config = summary.Configs.Single();
            var configModel = model.Configs.Single();
            Assert.Equal(configModel.Type.Name.ToLowerCamelCase(), config.Id);
            Assert.Equal(configModel.ConfigurationDisplayName, config.DisplayName);
            Assert.Equal(configModel.ConfigurationDescription, config.Description);
        }

        [Fact]
        public async Task Get_Returns403_IfNoReadClaim()
        {
            var testContext = TestHttpContextBuilder.CreateForPath("/")
                .WithClaims()
                .TestContext;

            await target.Handle(testContext, options);
            responseFactory.Verify(f => f.BuildStatusResponse(testContext, 403));                
        }

        [Fact]
        public async Task Get_Model_ReturnsConfigurationSetModelForClient()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{modelPath}/{clientId}/{typeof(SampleConfigSet).Name}")
                .WithClaims(readClaim).TestContext;
            var mappedModel = new ConfigurationSetModelPayload();
            modelPayloadMapper.Setup(m => m.Map(configCollection.GetConfigSetDefinition(typeof(SampleConfigSet)), It.Is<ConfigurationIdentity>(i => i.Client.Equals(expectedClient))))
                .ReturnsAsync(() => mappedModel);

            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildJsonResponse(testContext, mappedModel));
            
        }

        [Fact]
        public async Task Get_Value_ReturnsConfigurationEditModelForClient()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{valuePath}/{clientId}/{typeof(SampleConfig).Name}").WithClaims(readClaim).TestContext;
            var configInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), new ConfigurationIdentity(expectedClient, new Version(1, 0)));
            configInstanceRouter.Setup(r => r.GetConfigInstanceOrDefault(expectedClient, typeof(SampleConfig).Name))
                .ReturnsAsync(() => configInstance);
            var mappedModel = new object();
            configurationEditModelMapper.Setup(m => m.MapToEditConfig(configInstance,(configCollection.GetConfigSetDefinition(typeof(SampleConfigSet)))))
                .Returns(() => mappedModel);

            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildJsonResponse(testContext, mappedModel));

        }

        [Fact]
        public async Task Post_Value_ReturnsCommandResultFromBus()
        {
            var stringValue = "Hello";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{valuePath}/{clientId}/{typeof(SampleConfig).Name}")
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
    }
}
