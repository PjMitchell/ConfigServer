using ConfigServer.Server;
using System;
using Xunit;

namespace ConfigServer.Core.Tests
{
    public class ConfigurationClassPropertyBuilderTests
    {
        private readonly ConfigurationModelBuilder<ParentTestClass, TestConfigSet> target;
        private const string nestedClassProperty = nameof(ParentTestClass.NestedClass);
        private const string dateTimeProperty = nameof(NestedTestClass.DateTimeProperty);



        public ConfigurationClassPropertyBuilderTests()
        {
            target = new ConfigurationModelBuilder<ParentTestClass, TestConfigSet>(new ConfigurationModel<ParentTestClass, TestConfigSet>(nameof(TestConfigSet.ParentClass), c => c.ParentClass, (set, c) => set.ParentClass = c));
        }

        [Fact]
        public void CanBuildModelDefinition_Property()
        {
            target.Property(x => x.NestedClass);
            var result = target.Build();

            Assert.True(result.ConfigurationProperties.ContainsKey(nestedClassProperty));
            Assert.Equal(nestedClassProperty, result.ConfigurationProperties[nestedClassProperty].ConfigurationPropertyName);

        }

        [Fact]
        public void CanBuildModelDefinition_PropertyWithNameAndDescription()
        {
            var name = "A Name";
            var description = "A Discription";

            target.Property(x => x.NestedClass)
                .WithDisplayName(name)
                .WithDescription(description);
            var result = target.Build();

            Assert.Equal(description, result.ConfigurationProperties[nestedClassProperty].PropertyDescription);
            Assert.Equal(name, result.ConfigurationProperties[nestedClassProperty].PropertyDisplayName);
        }

        [Fact]
        public void CanBuildModelDefinition_WithDefaultProperties()
        {
            target.Property(x => x.NestedClass);
            var result = target.Build();
            var nestedClassPropertyDefinition = (ConfigurationClassPropertyDefinition)result.ConfigurationProperties[nestedClassProperty];
            Assert.True(nestedClassPropertyDefinition.ConfigurationProperties.ContainsKey(dateTimeProperty));
            Assert.Equal(dateTimeProperty, nestedClassPropertyDefinition.ConfigurationProperties[dateTimeProperty].ConfigurationPropertyName);

        }


        private class TestConfigSet : ConfigurationSet<TestConfigSet>
        {
            public Config<ParentTestClass> ParentClass { get; set; }
        }

        private class ParentTestClass
        {
            public NestedTestClass NestedClass { get; set; }
        }

        private class NestedTestClass
        {
            public DateTime DateTimeProperty { get; set; }
        }
    }
    
}
