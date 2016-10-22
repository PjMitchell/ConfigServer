using ConfigServer.Sample.Models;
using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigurationEditPayloadMapperTests
    {
        private IConfigurationEditPayloadMapper target;
        private ConfigurationSetModel definition;
        private SampleConfig sample;
        private JObject updatedObject;
        private SampleConfig updatedSample;



        public ConfigurationEditPayloadMapperTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<IOptionProvider, OptionProvider>();


            target = new ConfigurationEditPayloadMapper(serviceCollection.BuildServiceProvider(), new PropertyTypeProvider());
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
            updatedValue.MoarOptions = updatedSample.MoarOptions.Select(s=> s.Id).ToList();
            updatedValue.ListOfConfigs = updatedSample.ListOfConfigs;
            var serilaisationSetting = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            var json = JsonConvert.SerializeObject(updatedValue, serilaisationSetting);
            updatedObject = JObject.Parse(json);
        }

        #region MapToEdit
        [Fact]
        public void MapsRegularValues()
        {          
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            Assert.Equal(sample.Choice, response.Choice);
            Assert.Equal(sample.IsLlamaFarmer, response.IsLlamaFarmer);
            Assert.Equal(sample.Decimal, response.Decimal);
            Assert.Equal(sample.LlamaCapacity, response.LlamaCapacity);
            Assert.Equal(sample.Name, response.Name);
        }

        [Fact]
        public void MapsOptionValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            Assert.Equal(sample.Option.Id.ToString(), response.Option);
        }

        [Fact]
        public void MapsOptionsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            var expected = sample.MoarOptions.Select(s => s.Id.ToString()).ToList();
            Assert.Equal(expected, response.MoarOptions);
        }

        [Fact]
        public void MapsListOfConfigsValues()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;
            Assert.Equal(1, listOfConfigs.Count());
            Assert.Equal(sample.ListOfConfigs[0].Name, listOfConfigs.First().Name);
            Assert.Equal(sample.ListOfConfigs[0].Value, listOfConfigs.First().Value);

        }

        [Fact]
        public void MapsNewObject()
        {
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(new SampleConfig()), definition);
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;

            var moarOptions = response.MoarOptions as IEnumerable<dynamic>;
            Assert.Equal(0, listOfConfigs.Count());
            Assert.Equal(0, moarOptions.Count());
        }
        #endregion

        #region MapToEdit
        [Fact]
        public void UpdatesRegularValues()
        {
            var response = target.UpdateConfigurationInstance(new ConfigInstance<SampleConfig>(sample),updatedObject, definition);
            var result = (SampleConfig)response.GetConfiguration();
            Assert.Equal(updatedSample.Choice, result.Choice);
            Assert.Equal(updatedSample.IsLlamaFarmer, result.IsLlamaFarmer);
            Assert.Equal(updatedSample.Decimal, result.Decimal);
            Assert.Equal(updatedSample.LlamaCapacity, result.LlamaCapacity);
            Assert.Equal(updatedSample.Name, result.Name);
        }

        [Fact]
        public void UpdatesOptionValues()
        {
            var response = target.UpdateConfigurationInstance(new ConfigInstance<SampleConfig>(sample), updatedObject, definition);
            var result = (SampleConfig)response.GetConfiguration();
            Assert.Equal(updatedSample.Option.Description, result.Option.Description);
        }

        [Fact]
        public void UpdatesOptionsValues()
        {
            var response = target.UpdateConfigurationInstance(new ConfigInstance<SampleConfig>(sample), updatedObject, definition);
            var result = (SampleConfig)response.GetConfiguration();
            Assert.Equal(updatedSample.MoarOptions.Count, result.MoarOptions.Count);
            Assert.Equal(updatedSample.MoarOptions.Select(s=>s.Description), result.MoarOptions.Select(s => s.Description));
        }
        [Fact]
        public void UpdatesListOfConfigsValues()
        {
            var response = target.UpdateConfigurationInstance(new ConfigInstance<SampleConfig>(sample), updatedObject, definition);
            var result = (SampleConfig)response.GetConfiguration();
            Assert.Equal(1, result.ListOfConfigs.Count);
            Assert.Equal(updatedSample.ListOfConfigs[0].Name, result.ListOfConfigs[0].Name);
            Assert.Equal(updatedSample.ListOfConfigs[0].Value, result.ListOfConfigs[0].Value);

        }
        #endregion
    }
}
