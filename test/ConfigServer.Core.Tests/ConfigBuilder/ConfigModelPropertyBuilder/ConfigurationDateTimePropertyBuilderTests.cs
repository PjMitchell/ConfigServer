using ConfigServer.Server;
using System;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationDateTimePropertyBuilderTests
    {
        private readonly ConfigurationModelBuilder<DateTimeTestClass, TestConfigSet> target;

        public ConfigurationDateTimePropertyBuilderTests()
        {
            target = new ConfigurationModelBuilder<DateTimeTestClass, TestConfigSet>(new ConfigurationModel<DateTimeTestClass, TestConfigSet>(nameof(TestConfigSet.DateTime), c=> c.DateTime));
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.Property(x => x.DateTimeProperty);
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nameof(DateTimeTestClass.DateTimeProperty)));
            Assert.Equal(nameof(DateTimeTestClass.DateTimeProperty), result.ConfigurationProperties[nameof(DateTimeTestClass.DateTimeProperty)].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.Property(x => x.DateTimeProperty)
                .WithDisplayName(name)
                .WithDescription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(DateTimeTestClass.DateTimeProperty)].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nameof(DateTimeTestClass.DateTimeProperty)].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithDefaultValidation()
        {
            target.Property(x => x.DateTimeProperty);
            var result = target.Build();

            Assert.Null(GetDateTimeProperty(result).ValidationRules.Max);
            Assert.Null(GetDateTimeProperty(result).ValidationRules.Min);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMaxValueValidation()
        {
            var max = new DateTime(2013, 10,10);
            target.Property(x => x.DateTimeProperty)
                .WithMaxValue(max);
            var result = target.Build();

            Assert.Equal(max, GetDateTimeProperty(result).ValidationRules.Max);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMinValueValidation()
        {
            var min = new DateTime(2013, 10, 10);
            target.Property(x => x.DateTimeProperty)
                .WithMinValue(min);
            var result = target.Build();

            Assert.Equal(min, GetDateTimeProperty(result).ValidationRules.Min);
        }

        private ConfigurationPrimitivePropertyModel GetDateTimeProperty(ConfigurationModel def)
        {
            return (ConfigurationPrimitivePropertyModel)def.ConfigurationProperties[nameof(DateTimeTestClass.DateTimeProperty)];
        }

        private class TestConfigSet : ConfigurationSet<TestConfigSet>
        {
            public Config<DateTimeTestClass> DateTime { get; set; }
        }

        private class DateTimeTestClass
        {
            public DateTime DateTimeProperty { get; set; }
        }
    }
}
