using ConfigServer.Server;
using System;
using System.Linq;
using Xunit;

namespace ConfigServer.Core.Tests.ConfigBuilder
{
    public class ConfigurationPropertyModelDefinitionFactoryTests
    {
        [Fact]
        public void CanBuildNestedClass()
        {
            var definitions = ConfigurationPropertyModelDefinitionFactory.GetDefaultConfigProperties(typeof(ParentTestClass)).ToArray();
            Assert.Single(definitions);
            var definition = definitions[0];
            Assert.Equal(nameof(ParentTestClass.NestedClass), definition.Key);
            Assert.IsType<ConfigurationClassPropertyDefinition<NestedTestClass>>(definition.Value);
            var modelDefinition = (ConfigurationClassPropertyDefinition<NestedTestClass>)definition.Value;
            Assert.Equal("Nested Class", modelDefinition.PropertyDisplayName);
            Assert.Null(modelDefinition.PropertyDescription);
            Assert.Equal(nameof(ParentTestClass.NestedClass), modelDefinition.ConfigurationPropertyName);
            Assert.Single(modelDefinition.ConfigurationProperties);
            var nestedDefinition = modelDefinition.ConfigurationProperties.Single();
            Assert.Equal(nameof(NestedTestClass.DateTimeProperty), nestedDefinition.Key);
        }

        private class ParentTestClass
        {
            [ConfigurationClass]
            public NestedTestClass NestedClass { get; set; }
        }

        private class NestedTestClass
        {
            public DateTime DateTimeProperty { get; set; }
        }
    }
}
