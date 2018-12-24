using ConfigServer.Server;
using ConfigServer.TestModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace ConfigServer.Core.Tests.ConfigBuilder
{
    public class ConfigSetOptionValueProviderTests
    {
        private const string expectedName = "Test option";
        private const string expectedDescription = "Test option";


        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_HasPropertyDefinition()
        {
            var propertyModel = BuildAndGetBaseModel(nameof(SampleConfig.Option));
            Assert.IsType<ConfigurationPropertyWithOptionValueModelDefinition<TestCofigSet, OptionFromConfigSet, int>>(propertyModel);
        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_HasDisplayAttributesMapped()
        {
            var propertyModel = BuildAndGetModel();
            Assert.Equal(expectedName, propertyModel.PropertyDisplayName);
            Assert.Equal(expectedDescription, propertyModel.PropertyDescription);

        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_Dependencies()
        {
            var propertyModel = BuildAndGetModel();
            var dependencies = propertyModel.GetDependencies().ToArray();
            Assert.Single(dependencies);
            Assert.Equal(nameof(TestCofigSet.Options), dependencies[0].PropertyPath);
            Assert.Equal(typeof(TestCofigSet), dependencies[0].ConfigurationSet);
        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_HasCorrectOptionProvider()
        {
            var propertyModel = BuildAndGetModel();
            var configSet = new TestCofigSet
            {
                Options = new OptionSet<OptionFromConfigSet>(new[] { new OptionFromConfigSet() }, i => i.Id.ToString(), i => i.Description)
            };
            var optionSet = propertyModel.GetOptionSet(configSet);
            Assert.Equal(configSet.Options, optionSet);

        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_List_HasPropertyDefinition()
        {
            var propertyModel = BuildAndGetBaseModel(nameof(SampleConfig.OptionList));
            Assert.IsType<ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TestCofigSet, OptionFromConfigSet, int, List<int>>>(propertyModel);
        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_List_HasDisplayAttributesMapped()
        {
            var propertyModel = BuildAndGetListModel();
            Assert.Equal(expectedName, propertyModel.PropertyDisplayName);
            Assert.Equal(expectedDescription, propertyModel.PropertyDescription);

        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_List_Dependencies()
        {
            var propertyModel = BuildAndGetListModel();
            var dependencies = propertyModel.GetDependencies().ToArray();
            Assert.Single(dependencies);
            Assert.Equal(nameof(TestCofigSet.Options), dependencies[0].PropertyPath);
            Assert.Equal(typeof(TestCofigSet), dependencies[0].ConfigurationSet);
        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_List_HasCorrectOptionProvider()
        {
            var propertyModel = BuildAndGetListModel();
            var configSet = new TestCofigSet
            {
                Options = new OptionSet<OptionFromConfigSet>(new[] { new OptionFromConfigSet() }, i => i.Id.ToString(), i => i.Description)
            };
            var optionSet = propertyModel.GetOptionSet(configSet);
            Assert.Equal(configSet.Options, optionSet);

        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_Array_HasPropertyDefinition()
        {
            var propertyModel = BuildAndGetBaseModel(nameof(SampleConfig.OptionArray));
            Assert.IsType<ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TestCofigSet, OptionFromConfigSet, int, int[]>>(propertyModel);
        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_Array_HasDisplayAttributesMapped()
        {
            var propertyModel = BuildAndGetArrayModel();
            Assert.Equal(expectedName, propertyModel.PropertyDisplayName);
            Assert.Equal(expectedDescription, propertyModel.PropertyDescription);

        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_Array_Dependencies()
        {
            var propertyModel = BuildAndGetArrayModel();
            var dependencies = propertyModel.GetDependencies().ToArray();
            Assert.Single(dependencies);
            Assert.Equal(nameof(TestCofigSet.Options), dependencies[0].PropertyPath);
            Assert.Equal(typeof(TestCofigSet), dependencies[0].ConfigurationSet);
        }

        [Fact]
        public void OptionProviderAttributeBuildOptionProvider_Array_HasCorrectOptionProvider()
        {
            var propertyModel = BuildAndGetArrayModel();
            var configSet = new TestCofigSet
            {
                Options = new OptionSet<OptionFromConfigSet>(new[] { new OptionFromConfigSet() }, i => i.Id.ToString(), i => i.Description)
            };
            var optionSet = propertyModel.GetOptionSet(configSet);
            Assert.Equal(configSet.Options, optionSet);

        }
        private ConfigurationPropertyWithOptionValueModelDefinition<TestCofigSet, OptionFromConfigSet, int> BuildAndGetModel() => (ConfigurationPropertyWithOptionValueModelDefinition<TestCofigSet, OptionFromConfigSet,int>)BuildAndGetBaseModel(nameof(SampleConfig.Option));

        private ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TestCofigSet, OptionFromConfigSet,int, List<int>> BuildAndGetListModel() => (ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TestCofigSet, OptionFromConfigSet, int, List<int>>)BuildAndGetBaseModel(nameof(SampleConfig.OptionList));

        private ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TestCofigSet, OptionFromConfigSet, int, int[]> BuildAndGetArrayModel() => (ConfigurationPropertyWithMultipleOptionValuesModelDefinition<TestCofigSet, OptionFromConfigSet, int, int[]>)BuildAndGetBaseModel(nameof(SampleConfig.OptionArray));

        private ConfigurationPropertyModelBase BuildAndGetBaseModel(string property)
        {
            var configsetModel = new TestCofigSet().BuildConfigurationSetModel();

            var configModel = configsetModel.Get<SampleConfig>();
            var propertyModel = configModel.ConfigurationProperties[property];
            return propertyModel;
        }


        private class TestCofigSet : ConfigurationSet<TestCofigSet>
        {
            public OptionSet<OptionFromConfigSet> Options { get; set; }
            public Config<SampleConfig> Config { get; set; }
        }

        private class SampleConfig
        {
            [Display(Name = expectedName, Description = expectedDescription)]
            [OptionValue(typeof(TestOptionValueProvider))]
            public int Option { get; set; }
            [Display(Name = expectedName, Description = expectedDescription)]
            [OptionValue(typeof(TestOptionValueProvider))]
            public int[] OptionArray { get; set; }
            [Display(Name = expectedName, Description = expectedDescription)]
            [OptionValue(typeof(TestOptionValueProvider))]
            public List<int> OptionList { get; set; }
        }

        private class TestOptionValueProvider : ConfigurationSetOptionValueProvider<TestCofigSet, OptionFromConfigSet, int>
        {
            public TestOptionValueProvider() : base((TestCofigSet set) => set.Options, (OptionFromConfigSet o) => o.Id)
            {

            }
        }

    }
}
