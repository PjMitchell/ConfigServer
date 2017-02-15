using ConfigServer.Server;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationIntergerPropertyDefinitionTests
    {
        private readonly ConfigurationModelBuilder<IntergerTestClass, TestConfigSet> target;

        public ConfigurationIntergerPropertyDefinitionTests()
        {
            target = new ConfigurationModelBuilder<IntergerTestClass, TestConfigSet>(new ConfigurationModel<IntergerTestClass, TestConfigSet>(nameof(TestConfigSet.Integer), p=> p.Integer));
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.Property(x => x.IntProperty);
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nameof(SimpleConfig.IntProperty)));
            Assert.Equal(nameof(IntergerTestClass.IntProperty), result.ConfigurationProperties[nameof(IntergerTestClass.IntProperty)].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.Property(x => x.IntProperty)
                .WithDisplayName(name)
                .WithDescription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(IntergerTestClass.IntProperty)].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nameof(IntergerTestClass.IntProperty)].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithDefaultValidation()
        {
            target.Property(x => x.IntProperty);
            var result = target.Build();

            Assert.Null(GetIntProperty(result).ValidationRules.Max);
            Assert.Null(GetIntProperty(result).ValidationRules.Min);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMaxValueValidation()
        {
            var max = 10;
            target.Property(x => x.IntProperty)
                .WithMaxValue(10);
            var result = target.Build();

            Assert.Equal(max, GetIntProperty(result).ValidationRules.Max);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMinValueValidation()
        {
            var min = 10;
            target.Property(x => x.IntProperty)
                .WithMinValue(10);
            var result = target.Build();

            Assert.Equal(min, GetIntProperty(result).ValidationRules.Min);
        }

        private ConfigurationPrimitivePropertyModel GetIntProperty(ConfigurationModel def)
        {
            return (ConfigurationPrimitivePropertyModel)def.ConfigurationProperties[nameof(IntergerTestClass.IntProperty)];
        }

        private class TestConfigSet : ConfigurationSet<TestConfigSet>
        {
            public Config<IntergerTestClass> Integer { get; set; }
        }

        private class IntergerTestClass
        {
            public int IntProperty { get; set; }
        }

    }
}
