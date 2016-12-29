using ConfigServer.Server;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.ConfigBuilder
{
    public class ConfigurationPropertyWithOptionBuilderTests
    {
        private readonly ConfigurationModelBuilder<PropertyWithOptionTestClass> target;
        private readonly Mock<IServiceProvider> mockServiceProvider;

        public ConfigurationPropertyWithOptionBuilderTests()
        {
            target = new ConfigurationModelBuilder<PropertyWithOptionTestClass>(new ConfigurationModel(typeof(PropertyWithOptionTestClass)));
            mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(p => p.GetService(typeof(OptionProvider))).Returns(()=> new OptionProvider());
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.PropertyWithOptions(x => x.OptionProperty, (OptionProvider provider) => provider.Get(), option => option.IntKey, option => option.DisplayValue );
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nameof(PropertyWithOptionTestClass.OptionProperty)));
            Assert.Equal(nameof(PropertyWithOptionTestClass.OptionProperty), result.ConfigurationProperties[nameof(PropertyWithOptionTestClass.OptionProperty)].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.PropertyWithOptions(x => x.OptionProperty, (OptionProvider provider) => provider.Get(), option => option.IntKey, option => option.DisplayValue)
                .WithDisplayName(name)
                .WithDescription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(PropertyWithOptionTestClass.OptionProperty)].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nameof(PropertyWithOptionTestClass.OptionProperty)].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_OverwriteExistingConfiguration()
        {
            var name = "A Name";
            var description = "A Discription";

            target.PropertyWithOptions(x => x.OptionProperty, (OptionProvider provider) => provider.Get(), option => option.IntKey, option => option.DisplayValue)
                .WithDisplayName(name)
                .WithDescription(description);
            target.PropertyWithOptions(x => x.OptionProperty, (OptionProvider provider) => provider.Get(), option => option.IntKey, option => option.DisplayValue);
            var result = target.Build();

            Assert.Equal(null, result.ConfigurationProperties[nameof(PropertyWithOptionTestClass.OptionProperty)].PropertyDescription);
            Assert.Equal("Option Property", result.ConfigurationProperties[nameof(PropertyWithOptionTestClass.OptionProperty)].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_ThatCanReturnOptions()
        {
            var optionProvider = new OptionProvider();
            var expected = optionProvider.Get().ToList();
            target.PropertyWithOptions(x => x.OptionProperty, (OptionProvider provider) => provider.Get(), option => option.IntKey, option => option.DisplayValue);
            var result = GetPropertyWithOption(target.Build()).BuildOptionSet(mockServiceProvider.Object);
            var options = result.OptionSelections.ToList();

            Assert.Equal(expected.Count, options.Count);
            Assert.Equal(expected.Single(s=> s.IntKey == 2).DisplayValue, options.Single(s => s.Key == 2.ToString()).DisplayValue);

        }

        [Fact]
        public void CanBuildModelDefinition_ThatCanReturnOptionByKey_IfExists()
        {
            var optionProvider = new OptionProvider();
            var expected = optionProvider.Get().ToList();
            target.PropertyWithOptions(x => x.OptionProperty, (OptionProvider provider) => provider.Get(), opt => opt.IntKey, opt => opt.DisplayValue);
            var result = GetPropertyWithOption(target.Build()).BuildOptionSet(mockServiceProvider.Object);
            object output;
            var option = result.TryGetValue(2.ToString(), out output);

            Assert.True(option);
            var outputAsOption = output as OptionTestClass;
            Assert.NotNull(outputAsOption);
            Assert.Equal(expected.Single(s => s.IntKey == 2).DisplayValue, outputAsOption.DisplayValue);

        }

        private ConfigurationPropertyWithOptionsModelDefinition GetPropertyWithOption(ConfigurationModel def)
        {
            return (ConfigurationPropertyWithOptionsModelDefinition)def.ConfigurationProperties[nameof(PropertyWithOptionTestClass.OptionProperty)];
        }

        private class PropertyWithOptionTestClass
        {
            public OptionTestClass OptionProperty { get; set; }
        }

        private class OptionTestClass
        {
            public int IntKey { get; set; }
            public string StringKey { get; set; }
            public string DisplayValue { get; set; }
        }

        private class OptionProvider
        {
            public IEnumerable<OptionTestClass> Get()
            {
                return new List<OptionTestClass>
                {
                    new OptionTestClass { IntKey = 1, StringKey = "One", DisplayValue = "Bridge" },
                    new OptionTestClass { IntKey = 2, StringKey = "Two", DisplayValue = "Cribage" },
                    new OptionTestClass { IntKey = 3, StringKey = "Three", DisplayValue = "Snap" }

                };
            }
        }

    }
}
