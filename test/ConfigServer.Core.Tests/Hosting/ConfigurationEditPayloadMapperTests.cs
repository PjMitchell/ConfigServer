using ConfigServer.Sample.Models;
using ConfigServer.Server;
using ConfigServer.Server.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Hosting
{
    public class ConfigurationEditPayloadMapperTests
    {
        private IConfigurationEditPayloadMapper target;

        public ConfigurationEditPayloadMapperTests()
        {
            target = new ConfigurationEditPayloadMapper(new PropertyTypeProvider());
        }

        [Fact]
        public void MapsRegularValues()
        {
            var definition = new SampleConfigSet().BuildConfigurationSetModel();
            var sample = new SampleConfig
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
            var definition = new SampleConfigSet().BuildConfigurationSetModel();
            var sample = new SampleConfig
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
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            Assert.Equal(sample.Option.Id.ToString(), response.Option);
        }

        [Fact]
        public void MapsOptionsValues()
        {
            var definition = new SampleConfigSet().BuildConfigurationSetModel();
            var sample = new SampleConfig
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
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            var expected = sample.MoarOptions.Select(s => s.Id.ToString()).ToList();
            Assert.Equal(expected, response.MoarOptions);
        }

        [Fact]
        public void MapsListOfConfigsValues()
        {
            var definition = new SampleConfigSet().BuildConfigurationSetModel();
            var sample = new SampleConfig
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
            var response = (dynamic)target.MapToEditConfig(new ConfigInstance<SampleConfig>(sample), definition);
            var listOfConfigs = response.ListOfConfigs as IEnumerable<dynamic>;

            var expected = sample.MoarOptions.Select(s => s.Id.ToString()).ToList();
            Assert.Equal(1, listOfConfigs.Count());
            Assert.Equal(sample.ListOfConfigs[0].Name, listOfConfigs.First().Name);
            Assert.Equal(sample.ListOfConfigs[0].Value, listOfConfigs.First().Value);

        }
    }
}
