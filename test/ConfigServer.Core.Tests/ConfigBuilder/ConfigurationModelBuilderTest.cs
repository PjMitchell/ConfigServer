using ConfigServer.Server;
using Xunit;
namespace ConfigServer.Core.Tests
{
    public class ConfigurationModelBuilderTest
    {
        private readonly ConfigurationModelBuilder<SimpleConfig, SimpleConfigSet> target;

        public ConfigurationModelBuilderTest()
        {
            target = new ConfigurationModelBuilder<SimpleConfig, SimpleConfigSet>(new ConfigurationModel<SimpleConfig, SimpleConfigSet>(nameof(SimpleConfigSet.Config), c=> c.Config, (set, c) => set.Config = c));
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
            Assert.Equal("Int Property", result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDisplayName);
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

            target.Property(x => x.IntProperty).WithDescription(description);
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
                .WithDescription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDisplayName);
        }

        
    }
}
