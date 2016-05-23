using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace ConfigServer.Core.Tests
{
    public class ConfigurationModelBuilderTest
    {
        private readonly ConfigurationModelBuilder<SimpleConfig> target;

        public ConfigurationModelBuilderTest()
        {
            target = new ConfigurationModelBuilder<SimpleConfig>(new ConfigurationModelDefinition(typeof(SimpleConfig)));
        }


        [Fact]
        public void CanBuildModelDefinition()
        {
            var result = target.Build();
            Assert.NotNull(result);
            Assert.Equal(typeof(SimpleConfig),result.Type);
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.Property(x => x.IntProperty);
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nameof(SimpleConfig.IntProperty)));
            Assert.Equal(nameof(SimpleConfig.IntProperty), result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithName()
        {
            var name = "A Name";

            target.Property(x => x.IntProperty).WithDisplayName(name);
            var result = target.Build();

            Assert.Equal(name, result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithDescription()
        {

            var description = "A Discription";

            target.Property(x => x.IntProperty).WithDiscription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDescription);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.Property(x => x.IntProperty)
                .WithDisplayName(name)
                .WithDiscription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDisplayName);
        }

        
    }

    public class ConfigurationIntergerPropertyDefinition
    {
        private readonly ConfigurationModelBuilder<IntergerTestClass> target;

        public ConfigurationIntergerPropertyDefinition()
        {
            target = new ConfigurationModelBuilder<IntergerTestClass>(new ConfigurationModelDefinition(typeof(SimpleConfig)));
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
                .WithDiscription(description);
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
            long max = 10;
            target.Property(x => x.IntProperty)
                .WithMaxValue(10);
            var result = target.Build();

            Assert.Equal(max, GetIntProperty(result).ValidationRules.Max);
        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithMinValueValidation()
        {
            long min = 10;
            target.Property(x => x.IntProperty)
                .WithMinValue(10);
            var result = target.Build();

            Assert.Equal(min, GetIntProperty(result).ValidationRules.Min);
        }

        private ConfigurationPropertyDefinition GetIntProperty(ConfigurationModelDefinition def)
        {
            return def.ConfigurationProperties[nameof(IntergerTestClass.IntProperty)];
        }

        private class IntergerTestClass
        {
            public int IntProperty { get; set; }
        }

    }
}
