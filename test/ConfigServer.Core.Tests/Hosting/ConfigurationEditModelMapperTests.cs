using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Moq;
using ConfigServer.Core.Tests.TestModels;
using ConfigServer.TestModels;

namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigurationEditModelMapperTests
    {
        private IConfigurationEditModelMapper target;
        private Mock<IConfigurationSetService> configurationSet;
        private ConfigurationSetModel definition;
        private SampleConfig sample;
        private JObject updatedObject;
        private SampleConfig updatedSample;

        private readonly ConfigurationIdentity clientId = new ConfigurationIdentity(new ConfigurationClient("7aa7d5f0-90fb-420b-a906-d482428a0c44"), new Version(1, 0));


        public ConfigurationEditModelMapperTests()
        {
            configurationSet = new Mock<IConfigurationSetService>();
            configurationSet.Setup(r => r.GetConfigurationSet(typeof(SampleConfigSet), It.IsAny<ConfigurationIdentity>()))
                .ReturnsAsync(() => new SampleConfigSet { Options = new OptionSet<OptionFromConfigSet>(OptionFromConfigSet.Options, o => o.Id.ToString(), o => o.Description) });

            target = new ConfigurationEditModelMapper(new TestOptionSetFactory(), new PropertyTypeProvider(), configurationSet.Object);
            definition = new SampleConfigSet().BuildConfigurationSetModel();
            sample = new SampleConfig
            {
                Choice = Choice.OptionThree,
                IsLlamaFarmer = true,
                Decimal = 0.23m,
                StartDate = new DateTime(2013, 10, 10),
                LlamaCapacity = 47,
                Name = "Name 1",
                Option = OptionProvider.OptionOne,
                MoarOptions = new List<Option> { OptionProvider.OptionOne, OptionProvider.OptionThree },
                ListOfConfigs = new List<ListConfig> { new ListConfig { Name = "One", Value = 23 } },
                NestedClass = new NestedClass { Count = 23, Description = "Test" },
                ListOfInts = new List<int> { 1,2,3},
                ListOfStrings = new List<string> { "Hello", "World" },

            };

            updatedSample = new SampleConfig
            {
                Choice = Choice.OptionTwo,
                IsLlamaFarmer = false,
                Decimal = 0.213m,
                StartDate = new DateTime(2013, 10, 11),
                LlamaCapacity = 147,
                Name = "Name 2",
                Option = OptionProvider.OptionTwo,
                MoarOptions = new List<Option> { OptionProvider.OptionTwo, OptionProvider.OptionThree },
                ListOfConfigs = new List<ListConfig> { new ListConfig { Name = "Two plus Two", Value = 5 } },
                NestedClass = new NestedClass { Count = 37, Description = "Test2" },
                ListOfInts = new List<int> { 1, 3, 4 },
                ListOfStrings = new List<string> { "Bye", "World" },
            };
            dynamic updatedValue = new ExpandoObject();
            updatedValue.Choice = updatedSample.Choice;
            updatedValue.IsLlamaFarmer = updatedSample.IsLlamaFarmer;
            updatedValue.Decimal = updatedSample.Decimal;
            updatedValue.StartDate = updatedSample.StartDate;
            updatedValue.LlamaCapacity = updatedSample.LlamaCapacity;
            updatedValue.Name = updatedSample.Name;
            updatedValue.Option = updatedSample.Option.Id;
            updatedValue.MoarOptions = updatedSample.MoarOptions.Select(s => s.Id).ToList();
            updatedValue.ListOfConfigs = updatedSample.ListOfConfigs;
            updatedValue.NestedClass = updatedSample.NestedClass;
            updatedValue.ListOfInts = updatedSample.ListOfInts;
            updatedValue.ListOfStrings = updatedSample.ListOfStrings;


            var serilaisationSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(updatedValue, serilaisationSetting);
            updatedObject = JObject.Parse(json);
        }

        [Fact]
        public void MapsRegularValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition.Get<SampleConfig>());
            Assert.Equal(sample.Choice, response.Choice);
            Assert.Equal(sample.IsLlamaFarmer, response.IsLlamaFarmer);
            Assert.Equal(sample.Decimal, response.Decimal);
            Assert.Equal(sample.LlamaCapacity, response.LlamaCapacity);
            Assert.Equal(sample.Name, response.Name);
        }

        [Fact]
        public void MapsPrimitiveCollectionValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition.Get<SampleConfig>());
            Assert.Equal(sample.ListOfStrings, response.ListOfStrings);
            Assert.Equal(sample.ListOfInts, response.ListOfInts);
        }

        [Fact]
        public void MapsOptionValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition.Get<SampleConfig>());
            Assert.Equal(sample.Option.Id.ToString(), response.Option);
        }

        [Fact]
        public void MapsOptionsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition.Get<SampleConfig>());
            var expected = sample.MoarOptions.Select(s => s.Id.ToString()).ToList();
            Assert.Equal(expected, response.MoarOptions);
        }

        [Fact]
        public void MapsListOfConfigsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition.Get<SampleConfig>());
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;
            Assert.Single(listOfConfigs);
            Assert.Equal(sample.ListOfConfigs[0].Name, listOfConfigs.First().Name);
            Assert.Equal(sample.ListOfConfigs[0].Value, listOfConfigs.First().Value);

        }

        [Fact]
        public void MapsNewObject()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(new SampleConfig(), clientId), definition.Get<SampleConfig>());
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;

            var moarOptions = response.MoarOptions as IEnumerable<dynamic>;
            Assert.Empty(listOfConfigs);
            Assert.Empty(moarOptions);
        }

        [Fact]
        public void MapsNestedConfigsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition.Get<SampleConfig>());
            Assert.Equal(sample.NestedClass.Count, response.NestedClass.Count);
            Assert.Equal(sample.NestedClass.Description, response.NestedClass.Description);

        }
    }

}
