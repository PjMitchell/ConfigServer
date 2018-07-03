using ConfigServer.Server;
using ConfigServer.Server.Validation;
using ConfigServer.TestModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class UpdateConfigurationSetFromJsonUploadCommandHandlerTests
    {
        private readonly ConfigurationModelRegistry registry;
        private readonly ConfigurationSetModel model; 
        private readonly Mock<IConfigurationSetUploadMapper> configurationSetUploadMapper;
        private readonly Mock<IConfigRepository> configRepository;
        private readonly Mock<IConfigurationValidator> configurationValidator;
        private readonly Mock<IEventService> eventService;
        private readonly ICommandHandler<UpdateConfigurationSetFromJsonUploadCommand> target;
        private ConfigurationIdentity expectedIdentity;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";
        private const string jsonPayload = "{}";

        public UpdateConfigurationSetFromJsonUploadCommandHandlerTests()
        {
            expectedIdentity = new ConfigurationIdentity(new ConfigurationClient(clientId), new Version(1, 0));
            var configSet = new SampleConfigSet();
            registry = new ConfigurationModelRegistry();
            model = configSet.BuildConfigurationSetModel();
            registry.AddConfigurationSet(model);
            configurationSetUploadMapper = new Mock<IConfigurationSetUploadMapper>();
            configRepository = new Mock<IConfigRepository>();
            configurationValidator = new Mock<IConfigurationValidator>();
            configurationValidator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), expectedIdentity))
                .ReturnsAsync(() => ValidationResult.CreateValid());
            eventService = new Mock<IEventService>();
            target = new UpdateConfigurationSetFromJsonUploadCommandHandler(registry, configurationSetUploadMapper.Object, configRepository.Object, configurationValidator.Object, eventService.Object);
        }

        [Fact]
        public async Task CallsUpdateWithRegularConfig()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var options = new List<OptionFromConfigSet>
            {
                new OptionFromConfigSet{ Id = 1, Description = "One"}
            };
            
            var input = new UpdateConfigurationSetFromJsonUploadCommand(expectedIdentity, typeof(SampleConfigSet), jsonPayload);
            configurationSetUploadMapper.Setup(r => r.MapConfigurationSetUpload(input.JsonUpload, model))
                .Returns(() => BuildConfigs(config, options));

            var result = await target.Handle(input);
            Assert.True(result.IsSuccessful);
            configRepository.Verify(repo => repo.UpdateConfigAsync(It.Is<ConfigInstance>(c => DoesMatch(c, config))));
        }

        private bool DoesMatch(ConfigInstance c, SampleConfig testConfig)
        {
            if (c is ConfigInstance<SampleConfig> castConfig)
                return testConfig.LlamaCapacity == castConfig.Configuration.LlamaCapacity;
            return false;
        }

        [Fact]
        public async Task RaisesEvent()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var options = new List<OptionFromConfigSet>
            {
                new OptionFromConfigSet{ Id = 1, Description = "One"}
            };

            var input = new UpdateConfigurationSetFromJsonUploadCommand(expectedIdentity, typeof(SampleConfigSet), jsonPayload);
            configurationSetUploadMapper.Setup(r => r.MapConfigurationSetUpload(input.JsonUpload, model))
                .Returns(() => BuildConfigs(config, options));

            var result = await target.Handle(input);

            eventService.Verify(repo => repo.Publish(It.Is<ConfigurationUpdatedEvent>(c => c.ConfigurationType == typeof(SampleConfig) && c.Identity == expectedIdentity)));
        }
        [Fact]
        public async Task DoesNotCallsUpdateIfValidationFailed()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var options = new List<OptionFromConfigSet>
            {
                new OptionFromConfigSet{ Id = 1, Description = "One"}
            };

            var input = new UpdateConfigurationSetFromJsonUploadCommand(expectedIdentity, typeof(SampleConfigSet), jsonPayload);
            configurationSetUploadMapper.Setup(r => r.MapConfigurationSetUpload(input.JsonUpload, model))
                .Returns(() => BuildConfigs(config, options));
            configurationValidator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), expectedIdentity))
                .ReturnsAsync(() => new ValidationResult("Error"));
            var result = await target.Handle(input);

            configRepository.Verify(repo => repo.UpdateConfigAsync(It.IsAny<ConfigInstance>()),Times.Never);
        }
        [Fact]
        public async Task DoesNotPublishEventIfValidationFailed()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var options = new List<OptionFromConfigSet>
            {
                new OptionFromConfigSet{ Id = 1, Description = "One"}
            };

            var input = new UpdateConfigurationSetFromJsonUploadCommand(expectedIdentity, typeof(SampleConfigSet), jsonPayload);
            configurationSetUploadMapper.Setup(r => r.MapConfigurationSetUpload(input.JsonUpload, model))
                .Returns(() => BuildConfigs(config, options));
            configurationValidator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), expectedIdentity))
                .ReturnsAsync(() => new ValidationResult("Error"));
            var result = await target.Handle(input);

            eventService.Verify(ev => ev.Publish(It.IsAny<ConfigurationUpdatedEvent>()), Times.Never);
        }

        [Fact]
        public async Task ReturnFailedIfValidationFailed()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var options = new List<OptionFromConfigSet>
            {
                new OptionFromConfigSet{ Id = 1, Description = "One"}
            };

            var input = new UpdateConfigurationSetFromJsonUploadCommand(expectedIdentity, typeof(SampleConfigSet), jsonPayload);
            configurationSetUploadMapper.Setup(r => r.MapConfigurationSetUpload(input.JsonUpload, model))
                .Returns(() => BuildConfigs(config, options));
            var validationResult = new ValidationResult(new[] { "Error", "Error2" });

            configurationValidator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), expectedIdentity))
                .ReturnsAsync(() => validationResult);
            var result = await target.Handle(input);

            Assert.False(result.IsSuccessful);
            Assert.Equal(string.Join(Environment.NewLine,new ValidationResult(new[] { validationResult, validationResult }).Errors), result.ErrorMessage);
        }

        private IEnumerable<KeyValuePair<string, object>> BuildConfigs(SampleConfig config, List<OptionFromConfigSet> options)
        {
            yield return new KeyValuePair<string, object>(nameof(SampleConfigSet.SampleConfig), config);
            yield return new KeyValuePair<string, object>(nameof(SampleConfigSet.Options), options);
        }
    }
}
