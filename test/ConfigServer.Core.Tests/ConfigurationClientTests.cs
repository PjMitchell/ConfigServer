using Newtonsoft.Json;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationClientTests
    {
        private const string clientId = "46e9e680-f27f-4a14-bdb8-ecfaaf7541f2";
        [Fact]
        public void Equals_IsBasedOnClientId()
        {

            var client1 = new ConfigurationClient(clientId);
            var client2 = new ConfigurationClient(clientId);
            Assert.True(client1.Equals(client2));
        }

        [Fact]
        public void Equals_IsBasedOnClientId_IgnoresCase()
        {
            var client1 = new ConfigurationClient(clientId.ToUpper());
            var client2 = new ConfigurationClient(clientId);
            Assert.True(client1.Equals(client2));
        }

        [Fact]
        public void AppSetting_SerializeAsCaseInsensitive()
        {
            var client = new ConfigurationClient(clientId);
            var key = "key";
            var setting = new ConfigurationClientSetting { Key = key, Value = "Value" };
            client.Settings.Add(setting.Key, setting);
            var json = JsonConvert.SerializeObject(client);
            var result = JsonConvert.DeserializeObject<ConfigurationClient>(json);
            Assert.Single(result.Settings);
            Assert.True(result.Settings.ContainsKey(key));
            Assert.True(result.Settings.ContainsKey(key.ToUpper()));

        }
    }
}
