using ConfigServer.Client;
using ConfigServer.Sample.Models;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Client
{
    public class ConfigServerClientTest
    {
        private readonly IConfigServer target;
        private readonly ConfigurationRegistry collection;
        private readonly ConfigServerClientOptions options;
        private readonly Mock<IHttpClientWrapper> clientWrapper;
        private const string configRegisration = "AnotherConfigName";


        public ConfigServerClientTest()
        {
            collection = new ConfigurationRegistry();
            collection.AddRegistration(ConfigurationRegistration.Build<SimpleConfig>());
            collection.AddRegistration(ConfigurationRegistration.Build<SampleConfig>(configRegisration));
            options = new ConfigServerClientOptions();
            options.ClientId = "1234-5678-1234";
            options.ConfigServer = "https://test.com/Config";
            options.CacheOptions.IsDisabled = true;
            clientWrapper = new Mock<IHttpClientWrapper>();
            target = new ConfigServerClient(clientWrapper.Object, new NoCachingStrategy(), collection, options);
        }
        #region Object
        [Fact]
        public async Task BuildConfigAsync_ThrowsExceptionIfConfigNotInRegistry()
        {
            await Assert.ThrowsAsync<InvalidConfigurationException>(() => target.GetConfigAsync(typeof(WrongConfig)));
        }

        [Fact]
        public async Task BuildConfigAsync_CallsConfigurationServerWithCorrectUri()
        {
            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(BuildResponse(new SimpleConfig())));
            await target.GetConfigAsync(typeof(SimpleConfig));
            clientWrapper.Verify(r => r.GetAsync(It.Is<Uri>(u => Check(u))), Times.AtLeastOnce());
        }

        [Fact]
        public async Task BuildConfigAsync_CallsConfigurationServerWithCorrectUri_SpecifiedName()
        {
            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(BuildResponse(new SampleConfig())));
            await target.GetConfigAsync(typeof(SampleConfig));
            clientWrapper.Verify(r => r.GetAsync(It.Is<Uri>(u => CheckByName(u, configRegisration))), Times.AtLeastOnce());
        }

        [Fact]
        public async Task BuildConfigAsync_Throws_If404()
        {
            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)));
            await Assert.ThrowsAsync<ConfigServerCommunicationException>(() => target.GetConfigAsync(typeof(SimpleConfig)));
        }

        [Fact]
        public async Task BuildConfigAsync_BuildsConfigFromResult()
        {
            var config = new SimpleConfig
            {
                IntProperty = 23
            };

            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(BuildResponse(config)));
            var result = await target.GetConfigAsync(typeof(SimpleConfig));
            var castResult = result as SimpleConfig;
            Assert.NotNull(castResult);
            Assert.Equal(config.IntProperty, castResult.IntProperty);
        }
        #endregion
        #region Generic
        [Fact]
        public async Task BuildConfigAsync_T_ThrowsExceptionIfConfigNotInRegistry()
        {
            await Assert.ThrowsAsync<InvalidConfigurationException>(() => target.GetConfigAsync<WrongConfig>());
        }

        [Fact]
        public async Task BuildConfigAsync_T_CallsConfigurationServerWithCorrectUri()
        {
            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(()=> Task.FromResult(BuildResponse(new SimpleConfig())));
            await target.GetConfigAsync<SimpleConfig>();
            clientWrapper.Verify(r => r.GetAsync(It.Is<Uri>(u => Check(u))), Times.AtLeastOnce());
        }

        [Fact]
        public async Task BuildConfigAsync_T_CallsConfigurationServerWithCorrectUri_SpecifiedName()
        {
            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(BuildResponse(new SampleConfig())));
            await target.GetConfigAsync<SampleConfig>();
            clientWrapper.Verify(r => r.GetAsync(It.Is<Uri>(u => CheckByName(u, configRegisration))), Times.AtLeastOnce());
        }

        [Fact]
        public async Task BuildConfigAsync_T_Throws_If404()
        {
            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound)));
            await Assert.ThrowsAsync<ConfigServerCommunicationException>(() => target.GetConfigAsync<SimpleConfig>());
        }

        [Fact]
        public async Task BuildConfigAsync_T_BuildsConfigFromResult()
        {
            var config = new SimpleConfig
            {
                IntProperty = 23
            };

            clientWrapper.Setup(r => r.GetAsync(It.IsAny<Uri>()))
                .Returns(() => Task.FromResult(BuildResponse(config)));
            var result = await target.GetConfigAsync<SimpleConfig>();

            Assert.Equal(config.IntProperty, result.IntProperty);
        }
        #endregion


        private bool Check(Uri uri) => CheckByName(uri, typeof(SimpleConfig).Name);

        private bool CheckByName(Uri uri, string name)
        {
            var expectedUri = new Uri($"{options.ConfigServer}/{options.ClientId}/{name}");
            var result = uri.Equals(expectedUri);
            return result;
        }

        private HttpResponseMessage BuildResponse(object content)
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(content, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
            return response;
        }

        private class WrongConfig
        {
            int NotThis { get; set; }
        }

    }
}
