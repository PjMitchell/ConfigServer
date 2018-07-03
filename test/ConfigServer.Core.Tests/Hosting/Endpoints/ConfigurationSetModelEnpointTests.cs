using ConfigServer.Server;
using ConfigServer.TestModels;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting.Endpoints
{
    public class ConfigurationSetModelEnpointTests
    {
        private readonly Mock<IHttpResponseFactory> responseFactory;
        private readonly Mock<IConfigurationSetModelPayloadMapper> modelPayloadMapper;
        private readonly ConfigurationModelRegistry configCollection;
        private readonly Mock<IConfigurationClientService> configClientService;

        private ConfigurationClient expectedClient;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private ConfigServerOptions options;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;

        // Model/{ Client Id}/{ Configuration Set}
        // GET: Model for configuration set
        public ConfigurationSetModelEnpointTests()
        {
            expectedClient = new ConfigurationClient(clientId);
            responseFactory = new Mock<IHttpResponseFactory>();
            modelPayloadMapper = new Mock<IConfigurationSetModelPayloadMapper>();
            configCollection = new ConfigurationModelRegistry();
            var configSetModel = new ConfigurationSetModel<SampleConfigSet>("Sample", "Sample description");
            configSetModel.GetOrInitialize(set => set.SampleConfig);
            configCollection.AddConfigurationSet(configSetModel);
            configClientService = new Mock<IConfigurationClientService>();
            configClientService.Setup(s => s.GetClientOrDefault(clientId))
                .ReturnsAsync(expectedClient);
            target = new ConfigurationSetModelEndpoint(responseFactory.Object, modelPayloadMapper.Object, configCollection, configClientService.Object);
            options = new ConfigServerOptions();
        }

        [Fact]
        public async Task Get_Model_ReturnsConfigurationSetModelForClient()
        {
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{typeof(SampleConfigSet).Name}")
                .WithClaims(readClaim).TestContext;
            var mappedModel = new ConfigurationSetModelPayload();
            modelPayloadMapper.Setup(m => m.Map(configCollection.GetConfigSetDefinition(typeof(SampleConfigSet)), It.Is<ConfigurationIdentity>(i => i.Client.Equals(expectedClient))))
                .ReturnsAsync(() => mappedModel);

            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildJsonResponse(testContext, mappedModel));
            
        }

        [Fact]
        public async Task Get_Model_Returns403IfNoConfiguratorClaimOnClient()
        {
            expectedClient.ConfiguratorClaim = "Denied";
            var testContext = TestHttpContextBuilder.CreateForPath($"/{clientId}/{typeof(SampleConfigSet).Name}")
                .WithClaims(readClaim).TestContext;
            
            await target.Handle(testContext, options);
            responseFactory.Verify(v => v.BuildStatusResponse(testContext, 403));

        }

    }
}
