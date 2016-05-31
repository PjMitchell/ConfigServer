using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationFloatPropertyBuilderTests
    {
        private readonly ConfigurationModelBuilder<FloatTestClass> target;

        public ConfigurationFloatPropertyBuilderTests()
        {
            target = new ConfigurationModelBuilder<FloatTestClass>(new ConfigurationModelDefinition(typeof(SimpleConfig)));
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
                .WithDiscription(description);
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

        private ConfigurationPropertyDefinition GetFloatProperty(ConfigurationModelDefinition def)
        {
            return def.ConfigurationProperties[nameof(FloatTestClass.FloatProperty)];
        }

        private class FloatTestClass
        {
            public double FloatProperty { get; set; }
        }
    }
}
