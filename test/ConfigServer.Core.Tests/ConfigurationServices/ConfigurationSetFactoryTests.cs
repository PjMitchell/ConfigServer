using ConfigServer.Server;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Collections;
using ConfigServer.Sample.Models;
using ConfigServer.Core.Tests.TestModels;
using System;

namespace ConfigServer.Core.Tests.ConfigurationServices
{
    public class ConfigurationSetFactoryTests
    {
        readonly IConfigurationSetFactory target;
        Mock<IConfigProvider> mockConfigProvider;
        ConfigurationModelRegistry registry;
        ConfigurationIdentity identity;
        List<ExternalOption> defaultOptions;
        List<OptionDependentOnAnotherOption> additionalOption;
        SampleConfig defaultConfig;

        public ConfigurationSetFactoryTests()
        {
            identity = new ConfigurationIdentity(new ConfigurationClient("fbce468f-0950-4b5f-a7e1-8e24e746bb91"), new Version(1, 0));
            registry = new ConfigurationModelRegistry();
            registry.AddConfigurationSet(new TestConfiguationModule().BuildConfigurationSetModel());
            defaultOptions = new List<ExternalOption>
            {
                new ExternalOption { Id = 1, Description = "testOne"}
            };

            additionalOption = new List<OptionDependentOnAnotherOption>
            {
                new OptionDependentOnAnotherOption { Id = 1, ExternalOption = defaultOptions[0], Value =23.3}
            };
            mockConfigProvider = new Mock<IConfigProvider>();
            mockConfigProvider.Setup(test => test.GetCollectionAsync(typeof(ExternalOption), It.IsAny<ConfigurationIdentity>()))
                .Returns(() => Task.FromResult<IEnumerable>(defaultOptions));
            mockConfigProvider.Setup(test => test.GetCollectionAsync(typeof(OptionDependentOnAnotherOption), It.IsAny<ConfigurationIdentity>()))
                .Returns(() => Task.FromResult<IEnumerable>(additionalOption));
            defaultConfig = new SampleConfig
            {
                LlamaCapacity = 23,
                ExternalOption = defaultOptions[0]                
            };
            var instance = new ConfigInstance<SampleConfig>(defaultConfig, identity);
            mockConfigProvider.Setup(test => test.GetAsync(typeof(SampleConfig), identity))
               .Returns(() => Task.FromResult<ConfigInstance>(instance));
            target = new ConfigurationSetFactory(mockConfigProvider.Object, new TestOptionSetFactory(), registry);
        }

        [Fact]
        public async Task Build_PopulatesIdentityProperty()
        {
            var configSet = await target.BuildConfigSet(typeof(TestConfiguationModule), identity);
            Assert.NotNull(configSet);
            Assert.Equal(identity,configSet.Instance);
        }

        [Fact]
        public async Task Build_PopulatesIdentityProperty_Generic()
        {
            var configSet = await target.BuildConfigSet<TestConfiguationModule>(identity);
            Assert.NotNull(configSet);
            Assert.Equal(identity, configSet.Instance);
        }

        [Fact]
        public async Task Build_PopulatesOption()
        {
            var options = new ExternalOption[]
            {
                new ExternalOption { Id = 1, Description = "testOne"},
                new ExternalOption { Id = 2, Description = "testTwo"}
            };
            mockConfigProvider.Setup(test => test.GetCollectionAsync(typeof(ExternalOption), identity))
               .Returns(() => Task.FromResult<IEnumerable>(options));

            var configSet = (TestConfiguationModule)await target.BuildConfigSet(typeof(TestConfiguationModule), identity);
            Assert.NotNull(configSet);
            Assert.Equal(options.Length, configSet.Options.Count);
            Assert.Equal(options[0].Description, configSet.Options["1"].Description);
            Assert.Equal(options[1].Description, configSet.Options["2"].Description);

        }

        [Fact]
        public async Task Build_PopulatesConfig()
        {
            var configSet = (TestConfiguationModule)await target.BuildConfigSet(typeof(TestConfiguationModule), identity);
            Assert.NotNull(configSet);
            Assert.Equal(defaultConfig.LlamaCapacity, configSet.TestConfig.Value.LlamaCapacity);
            Assert.Equal(defaultConfig.ExternalOption.Id, configSet.TestConfig.Value.ExternalOption.Id);
            Assert.Equal(defaultConfig.ExternalOption.Description, configSet.TestConfig.Value.ExternalOption.Description);
        }

        [Fact]
        public async Task Build_PopulatesConfig_WithCorrectOptionValue()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23,
                ExternalOption = new ExternalOption { Id = defaultOptions[0].Id, Description = "Not this one" }
            };
            var instance = new ConfigInstance<SampleConfig>(config, identity);
            mockConfigProvider.Setup(test => test.GetAsync(typeof(SampleConfig), identity))
                   .Returns(() => Task.FromResult<ConfigInstance>(instance));

            var configSet = (TestConfiguationModule)await target.BuildConfigSet(typeof(TestConfiguationModule), identity);
            Assert.NotNull(configSet);
            Assert.Equal(defaultConfig.LlamaCapacity, configSet.TestConfig.Value.LlamaCapacity);
            Assert.Equal(defaultConfig.ExternalOption.Id, configSet.TestConfig.Value.ExternalOption.Id);
            Assert.Equal(defaultConfig.ExternalOption.Description, configSet.TestConfig.Value.ExternalOption.Description);
        }

        [Fact]
        public async Task CheckOptionsAreUpdated()
        {
            var config = new SampleConfig
            {
                Option = new Option { Id = 1, Description = "Not the right description" },
                MoarOptions = new List<Option>
                {
                    new Option{ Id = 3, Description ="fail"},
                    OptionProvider.OptionTwo
                }
            };
            var instance = new ConfigInstance<SampleConfig>(config, identity);
            mockConfigProvider.Setup(test => test.GetAsync(typeof(SampleConfig), identity))
                   .Returns(() => Task.FromResult<ConfigInstance>(instance));
            var configSet = (TestConfiguationModule)await target.BuildConfigSet(typeof(TestConfiguationModule), identity);
            Assert.Equal(config.Option.Description, OptionProvider.OptionOne.Description);
            Assert.Equal(config.MoarOptions[0].Description, OptionProvider.OptionThree.Description);
            Assert.Equal(config.MoarOptions[1], OptionProvider.OptionTwo);
        }


        [Fact]
        public async Task CheckAdditionalOptionsAreUpdated()
        {
            additionalOption[0].ExternalOption =  new ExternalOption { Id = defaultOptions[0].Id, Description = "NotThis" };
            var configSet = (TestConfiguationModule)await target.BuildConfigSet(typeof(TestConfiguationModule), identity);
            Assert.Equal(additionalOption.Count, configSet.OptionDependentOnAnotherOption.Count);
            Assert.Equal(defaultOptions[0].Description, configSet.OptionDependentOnAnotherOption[additionalOption[0].Id.ToString()].ExternalOption.Description);
        }

        private class TestConfiguationModule : ConfigurationSet<TestConfiguationModule>
        {
            public OptionSet<ExternalOption> Options { get; set; }
            public OptionSet<Option> OptionsFromProvider { get; set; }

            public OptionSet<OptionDependentOnAnotherOption> OptionDependentOnAnotherOption { get; set; }
            public Config<SampleConfig> TestConfig { get; set; }

            protected override void OnModelCreation(ConfigurationSetModelBuilder<TestConfiguationModule> modelBuilder)
            {
                modelBuilder.Options(set => set.Options, r => r.Id, r => r.Description); 
                modelBuilder.Options(set => set.OptionsFromProvider, r => r.Id, r => r.Description, (IOptionProvider provider) => provider.GetOptions());

                var optionConfig = modelBuilder.Options(set => set.OptionDependentOnAnotherOption, r => r.Id, r => r)
                    .PropertyWithOption(r=> r.ExternalOption, (TestConfiguationModule option)=> option.Options);

                var configBuilder = modelBuilder.Config(set => set.TestConfig);
                configBuilder.PropertyWithOption(p => p.ExternalOption, (TestConfiguationModule provider) => provider.Options);
                configBuilder.PropertyWithOption(p => p.Option, (TestConfiguationModule provider) => provider.OptionsFromProvider);
                configBuilder.PropertyWithMultipleOptions(p => p.MoarOptions, (TestConfiguationModule provider) => provider.OptionsFromProvider);
            }
        }

        private class OptionDependentOnAnotherOption
        {
            public int Id { get; set; }
            public ExternalOption ExternalOption { get; set; }
            public double Value { get; set; }

            public override string ToString() => ExternalOption?.Description ?? string.Empty;
        }
    }
}
