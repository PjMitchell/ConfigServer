using System;
using ConfigServer.Server.Validation;
using ConfigServer.Core;
using System.Threading.Tasks;

namespace ConfigServer.Server
{
    internal class UpdateConfigurationFromJsonUploadCommandHandler : ICommandHandler<UpdateConfigurationFromJsonUploadCommand>
    {
        private readonly IConfigurationSetRegistry registry;
        private readonly IConfigurationService configurationService;
        private readonly IConfigRepository configRepository;
        private readonly IConfigurationValidator configurationValidator;
        private readonly IConfigurationUploadMapper configurationUploadMapper;
        private readonly IEventService eventService;

        public UpdateConfigurationFromJsonUploadCommandHandler(IConfigurationSetRegistry registry, IConfigurationService configurationService,IConfigRepository configRepository, IConfigurationValidator configurationValidator, IEventService eventService, IConfigurationUploadMapper configurationUploadMapper)
        {
            this.registry = registry;
            this.configurationService = configurationService;
            this.configRepository = configRepository;
            this.configurationValidator = configurationValidator;
            this.configurationUploadMapper = configurationUploadMapper;
            this.eventService = eventService;
        }

        public async Task<CommandResult> Handle(UpdateConfigurationFromJsonUploadCommand command)
        {
            var model = registry.GetConfigDefinition(command.ConfigurationType);
            var configInstance = configurationUploadMapper.MapToConfigInstance(command.JsonUpload,command.Identity, model);
            var validationResult = await configurationValidator.Validate(configInstance.GetConfiguration(), model, command.Identity);
            if (validationResult.IsValid)
            {
                await configRepository.UpdateConfigAsync(configInstance);
                await eventService.Publish(new ConfigurationUpdatedEvent(configInstance));
                return CommandResult.Success();
            }

            return CommandResult.Failure(string.Join(Environment.NewLine, validationResult.Errors));
        }

        private static ConfigInstance BuildConfigInstance(ConfigurationIdentity identity, ConfigurationModel model, object config)
        {
            ConfigInstance instance;
            if (model is ConfigurationOptionModel)
                instance = ConfigFactory.CreateGenericCollectionInstance(model.Type, identity);
            else
                instance = ConfigFactory.CreateGenericInstance(model.Type, identity);
            instance.SetConfiguration(config);
            return instance;
        }
    }
}
