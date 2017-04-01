using ConfigServer.Sample.Models;
using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using ConfigServer.Core.Tests.TestModels;
using Moq;

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

        private readonly ConfigurationIdentity clientId = new ConfigurationIdentity(new ConfigurationClient("7aa7d5f0-90fb-420b-a906-d482428a0c44"));


        public ConfigurationEditModelMapperTests()
        {
            configurationSet = new Mock<IConfigurationSetService>();
            configurationSet.Setup(r => r.GetConfigurationSet(typeof(SampleConfigSet), It.IsAny<ConfigurationIdentity>()))
                .ReturnsAsync(() => new SampleConfigSet { Options = new OptionSet<Option>(OptionProvider.Options, o => o.Id.ToString(), o => o.Description) });

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
                ListOfConfigs = new List<ListConfig> { new ListConfig { Name = "One", Value = 23 } }
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
                ListOfConfigs = new List<ListConfig> { new ListConfig { Name = "Two plus Two", Value = 5 } }
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
            var serilaisationSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(updatedValue, serilaisationSetting);
            updatedObject = JObject.Parse(json);
        }

        [Fact]
        public void MapsRegularValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition);
            Assert.Equal(sample.Choice, response.Choice);
            Assert.Equal(sample.IsLlamaFarmer, response.IsLlamaFarmer);
            Assert.Equal(sample.Decimal, response.Decimal);
            Assert.Equal(sample.LlamaCapacity, response.LlamaCapacity);
            Assert.Equal(sample.Name, response.Name);
        }

        [Fact]
        public void MapsOptionValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition);
            Assert.Equal(sample.Option.Id.ToString(), response.Option);
        }

        [Fact]
        public void MapsOptionsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition);
            var expected = sample.MoarOptions.Select(s => s.Id.ToString()).ToList();
            Assert.Equal(expected, response.MoarOptions);
        }

        [Fact]
        public void MapsListOfConfigsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample, clientId), definition);
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;
            Assert.Equal(1, listOfConfigs.Count());
            Assert.Equal(sample.ListOfConfigs[0].Name, listOfConfigs.First().Name);
            Assert.Equal(sample.ListOfConfigs[0].Value, listOfConfigs.First().Value);

        }

        [Fact]
        public void MapsNewObject()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(new SampleConfig(), clientId), definition);
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;

            var moarOptions = response.MoarOptions as IEnumerable<dynamic>;
            Assert.Equal(0, listOfConfigs.Count());
            Assert.Equal(0, moarOptions.Count());
        }
    }

}
