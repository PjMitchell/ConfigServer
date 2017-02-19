using Xunit;
using ConfigServer.Core;
using ConfigServer.Server;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationFloatPropertyBuilderTests
    {
        private readonly ConfigurationModelBuilder<FloatTestClass, TestConfigSet> target;

        public ConfigurationFloatPropertyBuilderTests()
        {
            target = new ConfigurationModelBuilder<FloatTestClass, TestConfigSet>(new ConfigurationModel<FloatTestClass, TestConfigSet>(nameof(TestConfigSet.Float), c => c.Float, (set, c) => set.Float = c));
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.Property(x => x.FloatProperty);
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nameof(FloatTestClass.FloatProperty)));
            Assert.Equal(nameof(FloatTestClass.FloatProperty), result.ConfigurationProperties[nameof(FloatTestClass.FloatProperty)].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.Property(x => x.FloatProperty)
                .WithDisplayName(name)
                .WithDescription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(FloatTestClass.FloatProperty)].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nameof(FloatTestClass.FloatProperty)].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithDefaultValidation()
        {
            target.Property(x => x.FloatProperty);
            var result = target.Build();

            Assert.Null(GetFloatProperty(result).ValidationRules.Max);
            Assert.Null(GetFloatProperty(result).ValidationRules.Min);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMaxValueValidation()
        {
            double max = 10;
            target.Property(x => x.FloatProperty)
                .WithMaxValue(10);
            var result = target.Build();

            Assert.Equal(max, GetFloatProperty(result).ValidationRules.Max);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMinValueValidation()
        {
            double min = 10;
            target.Property(x => x.FloatProperty)
                .WithMinValue(10);
            var result = target.Build();

            Assert.Equal(min, GetFloatProperty(result).ValidationRules.Min);
        }

        private ConfigurationPrimitivePropertyModel GetFloatProperty(ConfigurationModel def)
        {
            return (ConfigurationPrimitivePropertyModel)def.ConfigurationProperties[nameof(FloatTestClass.FloatProperty)];
        }

        private class TestConfigSet : ConfigurationSet<TestConfigSet>
        {
            public Config<FloatTestClass> Float { get; set; }
        }

        private class FloatTestClass
        {
            public double FloatProperty { get; set; }
        }
    }
}
