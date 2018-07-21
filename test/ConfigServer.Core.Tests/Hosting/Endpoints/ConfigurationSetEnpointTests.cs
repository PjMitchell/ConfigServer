using ConfigServer.Server;
using ConfigServer.TestModels;
using Microsoft.AspNetCore.Http;
using Moq;
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
        private readonly ConfigurationModelRegistry configCollection;
        private ConfigServerOptions options;
        private static readonly Claim readClaim = new Claim(ConfigServerConstants.ClientAdminClaimType, ConfigServerConstants.ConfiguratorClaimValue);
        private readonly IEndpoint target;

        // GET: Gets all configuration set summaries
        public ConfigurationSetEnpointTests()
        {

            responseFactory = new Mock<IHttpResponseFactory>();
            configCollection = new ConfigurationModelRegistry();
            var configSetModel = new ConfigurationSetModel<SampleConfigSet>("Sample", "Sample description");
            configSetModel.GetOrInitialize(set => set.SampleConfig);
            configCollection.AddConfigurationSet(configSetModel);

            target = new ConfigurationSetEndpoint(responseFactory.Object, configCollection);
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
            Assert.Single(summaries);
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
    }
}
