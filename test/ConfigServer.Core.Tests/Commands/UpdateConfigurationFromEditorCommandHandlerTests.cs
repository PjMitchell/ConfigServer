using ConfigServer.Sample.Models;
using ConfigServer.Server;
using ConfigServer.Server.Validation;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ConfigServer.Core.Tests.Commands
{
    public class UpdateConfigurationFromEditorCommandHandlerTests
    {
        private readonly Mock<IConfigurationService> configurationService;
        private readonly Mock<IConfigurationUpdatePayloadMapper> configurationUpdatePayloadMapper;
        private readonly Mock<IConfigurationSetRegistry> configSetRegistry;
        private readonly Mock<IConfigRepository> configRepository;
        private readonly Mock<IEventService> eventService;

        private readonly Mock<IConfigurationValidator> validator;
        private readonly ICommandHandler<UpdateConfigurationFromEditorCommand> target;
        private UpdateConfigurationFromEditorCommand command;
        private ConfigurationSetModel model;

        private const string clientId = "3E37AC18-A00F-47A5-B84E-C79E0823F6D4";

        public UpdateConfigurationFromEditorCommandHandlerTests()
        {
            command = new UpdateConfigurationFromEditorCommand(new ConfigurationIdentity(new ConfigurationClient(clientId)), typeof(SampleConfig), "{}");
            var configSetModel = new ConfigurationSetModel<SampleConfigSet>();
            configSetModel.GetOrInitialize(set => set.SampleConfig);
            model = configSetModel;
            configRepository = new Mock<IConfigRepository>();
            configurationUpdatePayloadMapper = new Mock<IConfigurationUpdatePayloadMapper>();
            configurationUpdatePayloadMapper.Setup(m => m.UpdateConfigurationInstance(It.IsAny<ConfigInstance>(),It.IsAny<string>(),It.IsAny<ConfigurationSetModel>()))
    .ReturnsAsync(() => new ConfigInstance<SampleConfig>());
            configSetRegistry = new Mock<IConfigurationSetRegistry>();
            configSetRegistry.Setup(r => r.GetConfigSetForConfig(typeof(SampleConfig)))
                .Returns(() => model);
            configurationService = new Mock<IConfigurationService>();
            eventService = new Mock<IEventService>();
            validator = new Mock<IConfigurationValidator>();
            validator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), It.IsAny<ConfigurationIdentity>()))
                .ReturnsAsync(() => ValidationResult.CreateValid());
            target = new UpdateConfigurationFromEditorCommandHandler(configurationService.Object, configurationUpdatePayloadMapper.Object, configSetRegistry.Object, validator.Object, configRepository.Object, eventService.Object);
        }

        [Fact]
        public async Task SavesParsedInstance()
        {
            var originalInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), command.Identity);
            var updatedInstance = new ConfigInstance<SampleConfig>(new SampleConfig { LlamaCapacity = 23}, command.Identity);
            configurationService.Setup(s => s.GetAsync(command.ConfigurationType, command.Identity))
                .ReturnsAsync(() => originalInstance);
            configSetRegistry.Setup(r => r.GetConfigSetForConfig(command.ConfigurationType))
                .Returns(() => model);
            configurationUpdatePayloadMapper.Setup(m => m.UpdateConfigurationInstance(originalInstance, command.ConfigurationAsJson, model))
                .ReturnsAsync(() => updatedInstance);
            await target.Handle(command);
            configRepository.Verify(r => r.UpdateConfigAsync(updatedInstance));

        }

        [Fact]
        public async Task RaisesEvent()
        {
            var originalInstance = new ConfigInstance<SampleConfig>(new SampleConfig(), command.Identity);
            var updatedInstance = new ConfigInstance<SampleConfig>(new SampleConfig { LlamaCapacity = 23 }, command.Identity);
            configurationService.Setup(s => s.GetAsync(command.ConfigurationType, command.Identity))
                .ReturnsAsync(() => originalInstance);
            configurationUpdatePayloadMapper.Setup(m => m.UpdateConfigurationInstance(originalInstance, command.ConfigurationAsJson, model))
                .ReturnsAsync(() => updatedInstance);
            await target.Handle(command);
            eventService.Verify(r => r.Publish(It.Is<ConfigurationUpdatedEvent>(e => e.Identity == command.Identity && e.ConfigurationType == command.ConfigurationType)));

        }

        [Fact]
        public async Task ReturnsSuccess()
        {
            var result = await target.Handle(command);
            Assert.True(result.IsSuccessful);

        }

        [Fact]
        public async Task ReturnsFailed_IfCannotParseConfig()
        {
            var exception = new ConfigModelParsingException("Message");
            configurationUpdatePayloadMapper.Setup(m => m.UpdateConfigurationInstance(It.IsAny<ConfigInstance>(), command.ConfigurationAsJson, It.IsAny<ConfigurationSetModel>()))
                .ThrowsAsync(exception);
            var result = await target.Handle(command);
            Assert.False(result.IsSuccessful);
            Assert.Equal(exception.Message, result.ErrorMessage);
        }

        [Fact]
        public async Task Calls_Validator()
        {
            var config = new ConfigInstance<SampleConfig>(new SampleConfig(), command.Identity);
            configurationUpdatePayloadMapper.Setup(m => m.UpdateConfigurationInstance(It.IsAny<ConfigInstance>(), command.ConfigurationAsJson, It.IsAny<ConfigurationSetModel>()))
                .ReturnsAsync(()=> config);
            var result = await target.Handle(command);
            validator.Verify(v => v.Validate(config.GetConfiguration(), model.Get<SampleConfig>(), command.Identity));
        }

        [Fact]
        public async Task ReturnsFailed_IfValidationFailed()
        {
            var validationResult = new ValidationResult(new[] { "Wrong", "Even more Wrong" });
           
            validator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), command.Identity))
                .ReturnsAsync(()=> validationResult);

            var result = await target.Handle(command);
            Assert.False(result.IsSuccessful);
            Assert.Equal(string.Join(Environment.NewLine,validationResult.Errors), result.ErrorMessage);

        }

        [Fact]
        public async Task InvalidComfig_DoesNotCallRepository()
        {
            var validationResult = new ValidationResult(new[] { "Wrong", "Even more Wrong" });

            validator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), command.Identity))
                .ReturnsAsync(() => validationResult);

            var result = await target.Handle(command);
            configRepository.Verify(r => r.UpdateConfigAsync(It.IsAny<ConfigInstance>()), Times.Never);

        }

        [Fact]
        public async Task InvalidComfig_DoesNotRaiseEvent()
        {
            var validationResult = new ValidationResult(new[] { "Wrong", "Even more Wrong" });

            validator.Setup(v => v.Validate(It.IsAny<object>(), It.IsAny<ConfigurationModel>(), command.Identity))
                .ReturnsAsync(() => validationResult);

            var result = await target.Handle(command);
            eventService.Verify(r => r.Publish(It.IsAny<ConfigurationUpdatedEvent>()), Times.Never);

        }
    }
}
