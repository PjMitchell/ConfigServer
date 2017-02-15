using ConfigServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigurationSetUploadMapperTests
    {
        private ConfigurationSetModel<TestConfigurationSet> model;
        private IConfigurationSetUploadMapper target;

        public ConfigurationSetUploadMapperTests()
        {
            model = new ConfigurationSetModel<TestConfigurationSet>();
            model.GetOrInitialize(nameof(TestConfigurationSet.ConfigOne), c=> c.ConfigOne);
            model.GetOrInitialize(nameof(TestConfigurationSet.ConfigTwo), c=> c.ConfigTwo);
            target = new ConfigurationSetUploadMapper();
        }

        [Fact]
        public void CanParseFullConfigurationSet()
        {
            var configSet = new TestConfigurationSet
            {
                ConfigOne = new Config<TestConfigOne>(new TestConfigOne { IntProperty = 2 }),
                ConfigTwo = new Config<TestConfigTwo>(new TestConfigTwo { StringProperty = "Test" })
            };
            var json = JsonConvert.SerializeObject(configSet);
            var jObject = JObject.Parse(json);
            var result = target.MapConfigurationSetUpload(jObject, model).ToDictionary(k=>k.Key, v=>v.Value);
            Assert.Equal(2, result.Count);
            var configOne = result[nameof(TestConfigurationSet.ConfigOne)] as TestConfigOne;
            Assert.NotNull(configOne);
            Assert.Equal(configSet.ConfigOne.Value.IntProperty, configOne.IntProperty);
            var configTwo = result[nameof(TestConfigurationSet.ConfigTwo)] as TestConfigTwo;
            Assert.NotNull(configTwo);
            Assert.Equal(configSet.ConfigTwo.Value.StringProperty, configTwo.StringProperty);
        }

        [Fact]
        public void ReturnsNullOnPropertyThatCannotBeParsed()
        {
            var configSet = new TestConfigurationSet
            {
                ConfigOne = new Config<TestConfigOne>(new TestConfigOne { IntProperty = 2 }),
                ConfigTwo = new Config<TestConfigTwo>(new TestConfigTwo { StringProperty = "Test" })
            };
            var jsonOne = JsonConvert.SerializeObject(configSet.ConfigOne);
            var json = $"{{\"ConfigOne\":{jsonOne},\"ConfigTwo\":{jsonOne}}}";

            var jObject = JObject.Parse(json);
            var result = target.MapConfigurationSetUpload(jObject, model).ToDictionary(k => k.Key, v => v.Value);
            Assert.Equal(2, result.Count);
            var configOne = result[nameof(TestConfigurationSet.ConfigOne)] as TestConfigOne;
            Assert.NotNull(configOne);
            Assert.Equal(configSet.ConfigOne.Value.IntProperty, configOne.IntProperty);
            var configTwo = result[nameof(TestConfigurationSet.ConfigTwo)] as TestConfigTwo;
            Assert.Null(configTwo);
        }

        [Fact]
        public void DoesNotReturnNullIfPropertyWasNotFound()
        {
            var configSet = new TestConfigurationSet
            {
                ConfigOne = new Config<TestConfigOne>(new TestConfigOne { IntProperty = 2 }),
                ConfigTwo = new Config<TestConfigTwo>(new TestConfigTwo { StringProperty = "Test" })
            };
            var jsonOne = JsonConvert.SerializeObject(configSet.ConfigOne);
            var json = $"{{\"ConfigOne\":{jsonOne}}}";

            var jObject = JObject.Parse(json);
            var result = target.MapConfigurationSetUpload(jObject, model).ToDictionary(k => k.Key, v => v.Value);
            Assert.Equal(1, result.Count);
            var configOne = result[nameof(TestConfigurationSet.ConfigOne)] as TestConfigOne;
            Assert.NotNull(configOne);
            Assert.Equal(configSet.ConfigOne.Value.IntProperty, configOne.IntProperty);
        }

        [Fact]
        public void IgnorePropertiesNotInConfigSet()
        {
            var configSet = new TestConfigurationSet
            {
                ConfigOne = new Config<TestConfigOne>(new TestConfigOne { IntProperty = 2 }),
                ConfigTwo = new Config<TestConfigTwo>(new TestConfigTwo { StringProperty = "Test" })
            };
            var jsonOne = JsonConvert.SerializeObject(configSet.ConfigOne);
            var json = $"{{\"ConfigOne\":{jsonOne},\"Meta\":{jsonOne}}}";

            var jObject = JObject.Parse(json);
            var result = target.MapConfigurationSetUpload(jObject, model).ToDictionary(k => k.Key, v => v.Value);
            Assert.Equal(1, result.Count);
            var configOne = result[nameof(TestConfigurationSet.ConfigOne)] as TestConfigOne;
            Assert.NotNull(configOne);
            Assert.Equal(configSet.ConfigOne.Value.IntProperty, configOne.IntProperty);
        }


        private class TestConfigOne
        {
            public int IntProperty { get; set; }
        }

        private class TestConfigTwo
        {
            public string StringProperty { get; set; }
        }

        private class TestConfigurationSet : ConfigurationSet<TestConfigurationSet>
        {
            public Config<TestConfigOne> ConfigOne { get; set; }
            public Config<TestConfigTwo> ConfigTwo { get; set; }
        }
    }
}
