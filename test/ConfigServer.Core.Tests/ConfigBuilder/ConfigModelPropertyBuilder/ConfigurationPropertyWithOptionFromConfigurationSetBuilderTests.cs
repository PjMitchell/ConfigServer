using ConfigServer.Server;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace ConfigServer.Core.Tests.ConfigBuilder
{
    public class ConfigurationPropertyWithOptionFromConfigurationSetBuilderTests
    {
        private readonly ConfigurationModelBuilder<TestConfig, TestConfiguationModule> target;
        private readonly ConfigurationIdentity configIdentity;
        public ConfigurationPropertyWithOptionFromConfigurationSetBuilderTests()
        {
            target = new ConfigurationModelBuilder<TestConfig, TestConfiguationModule>(new ConfigurationModel<TestConfig, TestConfiguationModule>(nameof(TestConfiguationModule.TestConfig), c=> c.TestConfig, (set, c) => set.TestConfig = c));
            configIdentity = new ConfigurationIdentity(new ConfigurationClient("TestId"));
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.PropertyWithOption(x => x.Option, (TestConfiguationModule provider) => provider.Options);
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nameof(TestConfig.Option)));
            Assert.Equal(nameof(TestConfig.Option), result.ConfigurationProperties[nameof(TestConfig.Option)].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.PropertyWithOption(x => x.Option, (TestConfiguationModule provider) => provider.Options)
               .WithDisplayName(name)
                .WithDescription(description);
            var result = target.Build();
            var def = GetPropertyWithOption(result);
            Assert.Equal(description, def.PropertyDescription);
            Assert.Equal(name, def.PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_OverwriteExistingConfiguration()
        {
            var name = "A Name";
            var description = "A Discription";

            target.PropertyWithOption(x => x.Option, (TestConfiguationModule provider) => provider.Options)
                .WithDisplayName(name)
                .WithDescription(description);
            target.PropertyWithOption(x => x.Option, (TestConfiguationModule provider) => provider.Options);
            var result = target.Build();
            var def = GetPropertyWithOption(result);
            Assert.Equal(null, def.PropertyDescription);
            Assert.Equal("Option", def.PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_SetsDependency()
        {
            target.PropertyWithOption(x => x.Option, (TestConfiguationModule provider) => provider.Options);
            var result = target.Build();
            var def = GetPropertyWithOption(result);
            var dependency = def.GetDependencies().ToList();
            Assert.Equal(1, dependency.Count);
            Assert.Equal(typeof(TestConfiguationModule), dependency[0].ConfigurationSet);
            Assert.Equal(nameof(TestConfiguationModule.Options), dependency[0].PropertyPath);
        }

        private ConfigurationPropertyWithOptionModelDefinition GetPropertyWithOption(ConfigurationModel def)
        {
            return (ConfigurationPropertyWithOptionModelDefinition)def.ConfigurationProperties[nameof(TestConfig.Option)];
        }



        private class TestConfiguationModule : ConfigurationSet<TestConfiguationModule>
        {
            public OptionSet<TestOption> Options { get; set; }
            public Config<TestConfig> TestConfig { get; set; }

            protected override void OnModelCreation(ConfigurationSetModelBuilder<TestConfiguationModule> modelBuilder)
            {
                //modelBuilder.Options(set => set.Options, r => r.Id, r => r.Description);
                //var configBuilder = modelBuilder.Config(set => set.TestConfig);
                //configBuilder.PropertyWithOptions
            }
        }

        private class TestConfig
        {
            public int Value { get; set; }
            public TestOption Option { get; set; }
        }

        private class TestOption
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

    }
}
