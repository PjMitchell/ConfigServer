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

    
}
