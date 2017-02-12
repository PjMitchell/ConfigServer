using ConfigServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationSetModelBuilderTests
    {
        private const string setName = "Sample name";
        private const string setDescription = "Sample Description";


        [Fact]
        public void CanBuildModel_WithBasicProperties()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var setModel = builder.Build();
            Assert.Equal(setName, setModel.Name);
            Assert.Equal(setDescription, setModel.Description);
            Assert.Equal(typeof(TestConfigSet), setModel.ConfigSetType);
        }
        #region config
        [Fact]
        public void CanBuildModel_WithConfig_HasDefaultValues()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            builder.Config(x=> x.Sample);
            var setModel = builder.Build();
            var configModel = setModel.Get<SimpleConfig>();
            Assert.Equal(typeof(SimpleConfig), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(typeof(SimpleConfig).Name, configModel.ConfigurationDisplayName);
            Assert.Equal(nameof(TestConfigSet.Sample), configModel.Name);
        }

        [Fact]
        public void CanBuildModel_WithConfig_WithName()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            builder.Config(x => x.Sample,name);
            var setModel = builder.Build();
            var configModel = setModel.Get<SimpleConfig>();
            Assert.Equal(typeof(SimpleConfig), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);

        }

        [Fact]
        public void CanBuildModel_WithConfig_WithNameDescription()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            var descript = "test descript";

            builder.Config(x=> x.Sample, name, descript);
            var setModel = builder.Build();
            var configModel = setModel.Get<SimpleConfig>();
            Assert.Equal(typeof(SimpleConfig), configModel.Type);
            Assert.Equal(descript, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);

        }

        #endregion
        #region Options
        [Fact]
        public void CanBuildModel_WithOption_HasDefaultValues()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            builder.Options(x => x.OptionsOne, x=> x.Id, x=> x.Value);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionOne>() as ConfigurationOptionModel;
            Assert.NotNull(configModel);
            Assert.Equal(typeof(OptionOne), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(typeof(OptionOne).Name, configModel.ConfigurationDisplayName);
            Assert.Equal(nameof(TestConfigSet.OptionsOne), configModel.Name);
        }

        [Fact]
        public void CanBuildModel_WithOption_WithName()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            builder.Options(x => x.OptionsOne, x => x.Id, x => x.Value, name);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionOne>();
            Assert.Equal(typeof(OptionOne), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);
        }

        [Fact]
        public void CanBuildModel_WithOption_WithNameDescription()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            var descript = "test descript";

            builder.Options(x => x.OptionsOne, x => x.Id, x => x.Value, name, descript);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionOne>();
            Assert.Equal(typeof(OptionOne), configModel.Type);
            Assert.Equal(descript, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);
        }

        [Fact]
        public void CanBuildModel_WithOption_StringKey_HasDefaultValues()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            builder.Options(x => x.OptionsTwo, x => x.Id, x => x.Value);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionTwo>() as ConfigurationOptionModel;
            Assert.NotNull(configModel);
            Assert.Equal(typeof(OptionTwo), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(typeof(OptionTwo).Name, configModel.ConfigurationDisplayName);
            Assert.Equal(nameof(TestConfigSet.OptionsTwo), configModel.Name);
        }

        [Fact]
        public void CanBuildModel_WithOption_StringKey_WithName()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            builder.Options(x => x.OptionsTwo, x => x.Id, x => x.Value, name);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionTwo>();
            Assert.Equal(typeof(OptionTwo), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);
        }

        [Fact]
        public void CanBuildModel_WithOption_StringKey_WithNameDescription()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            var descript = "test descript";

            builder.Options(x => x.OptionsTwo, x => x.Id, x => x.Value, name, descript);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionTwo>();
            Assert.Equal(typeof(OptionTwo), configModel.Type);
            Assert.Equal(descript, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);
        }

        [Fact]
        public void CanBuildModel_WithOption_LongKey_HasDefaultValues()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            builder.Options(x => x.OptionsThree, x => x.Key, x => x.Description);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionThree>() as ConfigurationOptionModel;
            Assert.NotNull(configModel);
            Assert.Equal(typeof(OptionThree), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(typeof(OptionThree).Name, configModel.ConfigurationDisplayName);
            Assert.Equal(nameof(TestConfigSet.OptionsThree), configModel.Name);
        }

        [Fact]
        public void CanBuildModel_WithOption_LongKey_WithName()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            builder.Options(x => x.OptionsThree, x => x.Key, x => x.Description, name);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionThree>();
            Assert.Equal(typeof(OptionThree), configModel.Type);
            Assert.Equal(string.Empty, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);
        }

        [Fact]
        public void CanBuildModel_WithOption_LongKey_WithNameDescription()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            var name = "test";
            var descript = "test descript";

            builder.Options(x => x.OptionsThree, x => x.Key, x => x.Description, name, descript);
            var setModel = builder.Build();
            var configModel = setModel.Get<OptionThree>();
            Assert.Equal(typeof(OptionThree), configModel.Type);
            Assert.Equal(descript, configModel.ConfigurationDescription);
            Assert.Equal(name, configModel.ConfigurationDisplayName);
        }

        [Fact]
        public void CanBuildModel_WithOption_CanBuildOptionSet()
        {
            var builder = new ConfigurationSetModelBuilder<TestConfigSet>(setName, setDescription);
            builder.Options(x => x.OptionsOne, x => x.Id, x => x.Value);
            var setModel = builder.Build();
            var options = new List<OptionOne>
            {
                new OptionOne { Id = 1, Value = 12},
                new OptionOne { Id = 2, Value = 32}
            };
            var configModel = setModel.Get<OptionOne>() as ConfigurationOptionModel;
            var optionSet = configModel.BuildOptionSet(options) as OptionSet<OptionOne>;
            Assert.NotNull(optionSet);
            Assert.Equal(options.Count, optionSet.Count);
            Assert.Equal(options[0].Value, optionSet["1"].Value);
            Assert.Equal(options[1].Value, optionSet["2"].Value);

        }
        #endregion

        private class TestConfigSet : ConfigurationSet<TestConfigSet>
        {
            public OptionSet<OptionOne> OptionsOne { get; set; }
            public OptionSet<OptionTwo> OptionsTwo { get; set; }

            public OptionSet<OptionThree> OptionsThree { get; set; }

            public Config<SimpleConfig> Sample { get; set; }

            public TestConfigSet() : base(setName, setDescription)
            {

            }
        }

        private class OptionOne
        {
            public int Id { get; set; }
            public double Value { get; set; }
        }

        private class OptionTwo
        {
            public string Id { get; set; }
            public string Value { get; set; }
        }

        private class OptionThree
        {
            public long Key { get; set; }
            public string Description { get; set; }
        }
    }
}
