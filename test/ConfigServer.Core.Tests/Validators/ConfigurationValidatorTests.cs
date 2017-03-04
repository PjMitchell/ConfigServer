using ConfigServer.Sample.Models;
using ConfigServer.Server.Validation;
using ConfigServer.Server;
using System.Linq;
using Xunit;
using System;
using ConfigServer.Core.Tests.TestModels;
using System.Collections.Generic;
using Moq;
using System.Threading.Tasks;

namespace ConfigServer.Core.Tests.Validators
{
    public class ConfigurationValidatorTests
    {
        private readonly ConfigurationModelBuilder<SampleConfig, SampleConfigSet> modelBuilder;
        private readonly IConfigurationValidator target;
        private readonly Mock<IConfigurationSetService> service;
        private readonly ConfigurationIdentity configIdentity;
        public ConfigurationValidatorTests()
        {
            modelBuilder = new ConfigurationModelBuilder<SampleConfig, SampleConfigSet>(nameof(SampleConfigSet.SampleConfig), c=> c.SampleConfig);
            configIdentity = new ConfigurationIdentity("TestId");
            service = new Mock<IConfigurationSetService>();
            target = new ConfigurationValidator(new TestOptionSetFactory(), service.Object);
        }

        [Fact]
        public async Task FailsIfWrongType()
        {
            var model = modelBuilder.Build();
            var result = await target.Validate(this, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.InvalidConfigType(model.Type), result.Errors.Single());
        }

        [Fact]
        public async Task FailsIfNull()
        {
            var model = modelBuilder.Build();
            var result = await target.Validate(null, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.InvalidConfigType(model.Type), result.Errors.Single());
        }

        #region Min Property Validation
        [Fact]
        public async Task FailsIfLessthanMin_int()
        {
            var min = 3;
            var sample = new SampleConfig
            {
                LlamaCapacity = 2
            };
            modelBuilder.Property(k => k.LlamaCapacity).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.LessThanMin(nameof(SampleConfig.LlamaCapacity), min), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToMin_int()
        {
            var min = 3;
            var sample = new SampleConfig
            {
                LlamaCapacity = min
            };
            modelBuilder.Property(k => k.LlamaCapacity).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }

        [Fact]
        public async Task FailsIfLessthanMin_nullable_int()
        {
            var min = 3;
            var sample = new SampleConfig
            {
                SpareLlamaCapacity = 2
            };
            modelBuilder.Property(k => k.SpareLlamaCapacity).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.LessThanMin(nameof(SampleConfig.SpareLlamaCapacity), min), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfnull_nullable_int()
        {
            var min = 3;
            var sample = new SampleConfig
            {
                SpareLlamaCapacity = null
            };
            modelBuilder.Property(k => k.SpareLlamaCapacity).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task FailsIfLessthanMin_decimal()
        {
            var min = 3m;
            var sample = new SampleConfig
            {
                Decimal = 2m
            };
            modelBuilder.Property(k => k.Decimal).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.LessThanMin(nameof(SampleConfig.Decimal), min), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToMin_decimal()
        {
            var min = 3m;
            var sample = new SampleConfig
            {
                Decimal = min
            };
            modelBuilder.Property(k => k.Decimal).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }

        [Fact]
        public async Task FailsIfLessthanMin_date()
        {
            var min = new DateTime(2013, 10, 10);
            var sample = new SampleConfig
            {
                StartDate = min.AddDays(-1)
            };
            modelBuilder.Property(k => k.StartDate).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.LessThanMin(nameof(SampleConfig.StartDate), min), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToMin_date()
        {
            var min = new DateTime(2013, 10, 10);
            var sample = new SampleConfig
            {
                StartDate = min
            };
            modelBuilder.Property(k => k.StartDate).WithMinValue(min);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }
        #endregion

        #region Max Property Validation
        [Fact]
        public async Task FailsIfGreaterThanMax_int()
        {
            var max = 3;
            var sample = new SampleConfig
            {
                LlamaCapacity = 4
            };
            modelBuilder.Property(k => k.LlamaCapacity).WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.GreaterThanMax(nameof(SampleConfig.LlamaCapacity), max), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToMax_int()
        {
            var max = 3;
            var sample = new SampleConfig
            {
                LlamaCapacity = max
            };
            modelBuilder.Property(k => k.LlamaCapacity).WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }

        [Fact]
        public async Task FailsIfGreaterThanMax_decimal()
        {
            var max = 3m;
            var sample = new SampleConfig
            {
                Decimal = 4m
            };
            modelBuilder.Property(k => k.Decimal).WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.GreaterThanMax(nameof(SampleConfig.Decimal), max), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToMax_decimal()
        {
            var max = 3m;
            var sample = new SampleConfig
            {
                Decimal = max
            };
            modelBuilder.Property(k => k.Decimal).WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }

        [Fact]
        public async Task FailsIfGreaterThanMax_date()
        {
            var max = new DateTime(2013, 10, 10);
            var sample = new SampleConfig
            {
                StartDate = max.AddDays(1)
            };
            modelBuilder.Property(k => k.StartDate).WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.GreaterThanMax(nameof(SampleConfig.StartDate), max), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToMax_date()
        {
            var max = new DateTime(2013, 10, 10);
            var sample = new SampleConfig
            {
                StartDate = max
            };
            modelBuilder.Property(k => k.StartDate).WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }
        #endregion

        [Fact]
        public async Task FailsIfGreaterThanLength()
        {
            var max = 3;
            var sample = new SampleConfig
            {
                Name = "Four"
            };
            modelBuilder.Property(k => k.Name).WithMaxLength(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.GreaterThanMaxLength(nameof(SampleConfig.Name), max), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfEqualToLength()
        {
            var max = 3;
            var sample = new SampleConfig
            {
                Name = "two"
            };
            modelBuilder.Property(k => k.Name).WithMaxLength(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);

        }

        [Fact]
        public async Task FailsIfPatternDoesNotMatch()
        {
            var pattern = @"^\d{3}";
            var sample = new SampleConfig
            {
                Name = "9wt20"
            };
            modelBuilder.Property(k => k.Name).WithPattern(pattern);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.MatchesPattern(nameof(SampleConfig.Name), pattern), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfPaternMatches()
        {
            var pattern = @"^\d{3}";
            var sample = new SampleConfig
            {
                Name = "920wt"
            };
            modelBuilder.Property(k => k.Name).WithPattern(pattern);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task FailsIfOptionNotAvailable()
        {
            var sample = new SampleConfig
            {
                Option = new Option { Id = 5, Description = "Option Four" }
            };
            modelBuilder.PropertyWithOptions<SampleConfig,Option,OptionProvider>(p=> p.Option,provider => provider.GetOptions(),o=>o.Id, o=>o.Description);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.OptionNotFound(nameof(SampleConfig.Option)), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfOptionFound()
        {
            var sample = new SampleConfig
            {
                Option = OptionProvider.OptionOne
            };
            modelBuilder.PropertyWithOptions<SampleConfig, Option, OptionProvider>(p => p.Option, provider => provider.GetOptions(), o => o.Id, o => o.Description);

            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task FailsIfOneOptionNotAvailable()
        {
            var sample = new SampleConfig
            {
                MoarOptions = new List<Option>
                {
                    new Option { Id = 5, Description = "Option Four" },
                    OptionProvider.OptionOne
                }
            };
            modelBuilder.PropertyWithMulitpleOptions<SampleConfig, Option, OptionProvider>(p => p.MoarOptions, provider => provider.GetOptions(), o => o.Id, o => o.Description);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.OptionNotFound(nameof(SampleConfig.MoarOptions)), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfAllOptionsFound()
        {
            var sample = new SampleConfig
            {
                MoarOptions = new List<Option>
                {
                    OptionProvider.OptionThree,
                    OptionProvider.OptionOne
                }

            };
            modelBuilder.PropertyWithMulitpleOptions<SampleConfig, Option, OptionProvider>(p => p.MoarOptions, provider => provider.GetOptions(), o => o.Id, o => o.Description);

            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task FailsIfOneConfigsInCollectionIsInvalid()
        {
            var max = 4;
            var sample = new SampleConfig
            {
                ListOfConfigs = new List<ListConfig>
                {
                    new ListConfig { Name = "item one", Value=5 },
                    new ListConfig { Name = "item two", Value=2 }
                }
            };
            modelBuilder.Collection(p => p.ListOfConfigs)
                .Property(p => p.Value)
                .WithMaxValue(max);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.GreaterThanMax(nameof(ListConfig.Value), max), result.Errors.Single());

        }

        [Fact]
        public async Task SucceedsIfAllConfigsInCollectionIsValid()
        {
            var sample = new SampleConfig
            {
                ListOfConfigs = new List<ListConfig>
                {
                    new ListConfig { Name = "item one", Value=4 },
                    new ListConfig { Name = "item two", Value=2 }
                }
            };
            modelBuilder.Collection(p => p.ListOfConfigs)
                .Property(p => p.Value)
                .WithMaxValue(4);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task FailsIfDuplicateKeysInCollection()
        {
            var sample = new SampleConfig
            {
                ListOfConfigs = new List<ListConfig>
                {
                    new ListConfig { Name = "item one", Value=2 },
                    new ListConfig { Name = "item two", Value=2 }
                }
            };
            modelBuilder.Collection(p => p.ListOfConfigs)
                .WithUniqueKey(p => p.Value);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count());
            Assert.Equal(ValidationStrings.DuplicateKeys(nameof(SampleConfig.ListOfConfigs), 2), result.Errors.Single());
        }

        [Fact]
        public async Task SucceedsIfDuplicateKeysNotInCollection()
        {
            var sample = new SampleConfig
            {
                ListOfConfigs = new List<ListConfig>
                {
                    new ListConfig { Name = "item one", Value=2 },
                    new ListConfig { Name = "item two", Value=3 }
                }
            };
            modelBuilder.Collection(p => p.ListOfConfigs)
                .WithUniqueKey(p => p.Value);
            var model = modelBuilder.Build();
            var result = await target.Validate(sample, model, configIdentity);
            Assert.True(result.IsValid);
        }
    }
}
