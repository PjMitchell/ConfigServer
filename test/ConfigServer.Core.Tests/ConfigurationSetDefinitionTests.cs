using ConfigServer.Server;
using System;
using System.Linq;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationSetDefinitionTests
    {
        private readonly Type defaultType = typeof(SimpleConfigSet);

        [Fact]
        public void NewConfiguarationSetDefinition_ContainsNoDefinitions()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();

            Assert.Empty(result.Configs);
        }

        [Fact]
        public void CanGetOrInitializeByPropertyInfo()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();
            var gotValue = result.GetOrInitialize(typeof(SimpleConfigSet).GetProperty(nameof(SimpleConfigSet.Config)));
            Assert.Single(result.Configs);
            Assert.Equal(result.Configs.Single(), gotValue);
        }

        [Fact]
        public void CanGetOrInitializeByTypeDoesNotDuplicateInitialization()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();
            var gotValue = result.GetOrInitialize(c => c.Config);
            var gotValue2 = result.GetOrInitialize(c => c.Config);

            Assert.Single(result.Configs);
            Assert.True(ReferenceEquals(gotValue, gotValue2));
        }

        [Fact]
        public void CanGetOrInitializeByGenericType()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();
            var gotValue = result.GetOrInitialize<SimpleConfig>(c=> c.Config);
            Assert.Single(result.Configs);
            Assert.Equal(result.Configs.Single(), gotValue);
        }

        [Fact]
        public void CanGetExistingType()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();
            var gotValue = result.GetOrInitialize(c => c.Config);
            var gotValue2 = result.Get(typeof(SimpleConfig));

            Assert.True(ReferenceEquals(gotValue, gotValue2));
        }

        [Fact]
        public void CanGetExistingType_Generic()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();
            var gotValue = result.GetOrInitialize(c => c.Config);
            var gotValue2 = result.Get<SimpleConfig>();

            Assert.True(ReferenceEquals(gotValue, gotValue2));
        }

        [Fact]
        public void Throws_IfConfigTYpeNotFound()
        {
            var target = new ConfigurationSetModel<SimpleConfigSet>();
            Assert.Throws<ConfigurationModelNotFoundException>(()=> target.Get<SimpleConfig>());
        }

        [Fact]
        public void GetOrInitializePopulatesModelProperties()
        {
            var result = new ConfigurationSetModel<SimpleConfigSet>();
            var gotValue = result.GetOrInitialize(c => c.Config);

            Assert.Single(result.Configs);
            Assert.Equal(typeof(int), gotValue.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyType);
            Assert.Equal("Int Property", gotValue.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].PropertyDisplayName);
            Assert.Equal(nameof(SimpleConfig.IntProperty), gotValue.ConfigurationProperties[nameof(SimpleConfig.IntProperty)].ConfigurationPropertyName);

        }

    }
}
