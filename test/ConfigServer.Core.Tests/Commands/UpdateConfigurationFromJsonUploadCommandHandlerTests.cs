using ConfigServer.Sample.Models;
using ConfigServer.Server;
using ConfigServer.Server.Validation;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class UpdateConfigurationFromJsonUploadCommandHandlerTests
    {
        private readonly ConfigurationSetRegistry registry;
        private readonly Mock<IConfigurationService> configurationService;
        private readonly Mock<IConfigRepository> configRepository;
        private readonly Mock<IConfigurationValidator> configurationValidator;
        private readonly Mock<IEventService> eventService;
        private readonly ICommandHandler<UpdateConfigurationFromJsonUploadCommand> target;
        private ConfigurationIdentity expectedIdentity;
        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D9";

        public UpdateConfigurationFromJsonUploadCommandHandlerTests()
        {
            expectedIdentity = new ConfigurationIdentity(new ConfigurationClient(clientId), new Version(1, 0));
            var configSet = new SampleConfigSet();
            registry = new ConfigurationSetRegistry();
            registry.AddConfigurationSet(configSet.BuildConfigurationSetModel());
            configurationService = new Mock<IConfigurationService>();
            configRepository = new Mock<IConfigRepository>();
            configurationValidator = new Mock<IConfigurationValidator>();
            configurationValidator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), expectedIdentity))
                .ReturnsAsync(() => ValidationResult.CreateValid());
            eventService = new Mock<IEventService>();
            target = new UpdateConfigurationFromJsonUploadCommandHandler(registry,configurationService.Object, configRepository.Object, configurationValidator.Object, eventService.Object);
        }

        [Fact]
        public async Task CallsUpdateWithRegularConfig()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var existingInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), expectedIdentity);
            var input = new UpdateConfigurationFromJsonUploadCommand(expectedIdentity, typeof(SampleConfig), JsonConvert.SerializeObject(config));
            configurationService.Setup(r => r.GetAsync(input.ConfigurationType, input.Identity))
                .ReturnsAsync(() => existingInstance);

            var result = await target.Handle(input);
            Assert.True(result.IsSuccessful);
            configRepository.Verify(repo => repo.UpdateConfigAsync(It.Is<ConfigInstance>(c => c == existingInstance && config.LlamaCapacity == ((ConfigInstance<SampleConfig>)c).Configuration.LlamaCapacity)));
        }

        [Fact]
        public async Task CallsUpdateWithCollectionConfig()
        {
            var config = new List<Option>
            {
                new Option{ Id = 1, Description = "One"}
            };
            var existingInstance = new ConfigCollectionInstance<Option>(expectedIdentity);
            var input = new UpdateConfigurationFromJsonUploadCommand(expectedIdentity, typeof(SampleConfig), JsonConvert.SerializeObject(config));
            configurationService.Setup(r => r.GetAsync(input.ConfigurationType, input.Identity))
                .ReturnsAsync(() => existingInstance);

            var result = await target.Handle(input);
            Assert.True(result.IsSuccessful);
            configRepository.Verify(repo => repo.UpdateConfigAsync(It.Is<ConfigInstance>(c => c == existingInstance && config.Count == ((ConfigCollectionInstance<Option>)c).Configuration.Count)));
        }

        [Fact]
        public async Task RaisesEvent()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var existingInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), expectedIdentity);
            var input = new UpdateConfigurationFromJsonUploadCommand(expectedIdentity, typeof(SampleConfig), JsonConvert.SerializeObject(config));
            configurationService.Setup(r => r.GetAsync(input.ConfigurationType, input.Identity))
                .ReturnsAsync(() => existingInstance);

            var result = await target.Handle(input);

            eventService.Verify(repo => repo.Publish(It.Is<ConfigurationUpdatedEvent>(c => c.ConfigurationType == existingInstance.ConfigType && c.Identity == expectedIdentity)));
        }
        [Fact]
        public async Task DoesNotCallsUpdateIfValidationFailed()
        {
            var config = new SampleConfig
            {
                LlamaCapacity = 23
            };
            var existingInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), expectedIdentity);
            var input = new UpdateConfigurationFromJsonUploadCommand(expectedIdentity, typeof(SampleConfig), JsonConvert.SerializeObject(config));

            configurationService.Setup(r => r.GetAsync(input.ConfigurationType, input.Identity))
                .ReturnsAsync(() => existingInstance);
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
            var existingInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), expectedIdentity);
            var input = new UpdateConfigurationFromJsonUploadCommand(expectedIdentity, typeof(SampleConfig), JsonConvert.SerializeObject(config));

            configurationService.Setup(r => r.GetAsync(input.ConfigurationType, input.Identity))
                .ReturnsAsync(() => existingInstance);
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
            var existingInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), expectedIdentity);
            var input = new UpdateConfigurationFromJsonUploadCommand(expectedIdentity, typeof(SampleConfig), JsonConvert.SerializeObject(config));
            var validationResult = new ValidationResult(new[] { "Error", "Error2" });
            configurationService.Setup(r => r.GetAsync(input.ConfigurationType, input.Identity))
                .ReturnsAsync(() => existingInstance);
            configurationValidator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), expectedIdentity))
                .ReturnsAsync(() => validationResult);
            var result = await target.Handle(input);

            Assert.False(result.IsSuccessful);
            Assert.Equal(string.Join(Environment.NewLine, validationResult.Errors), result.ErrorMessage);
        }
    }
}
