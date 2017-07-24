using System;
using System.Threading.Tasks;
using ConfigServer.Core;
using ConfigServer.Server.Validation;

namespace ConfigServer.Server
{
    internal class UpdateConfigurationFromEditorCommandHandler : ICommandHandler<UpdateConfigurationFromEditorCommand>
    {
        private readonly IConfigurationService configurationService;
        private readonly IConfigurationUpdatePayloadMapper configurationUpdatePayloadMapper;
        private readonly IConfigurationModelRegistry configSetRegistry;
        private readonly IConfigurationValidator validator;
        private readonly IConfigRepository configRepository;
        private readonly IEventService eventService;
        public UpdateConfigurationFromEditorCommandHandler(IConfigurationService configurationService, IConfigurationUpdatePayloadMapper configurationUpdatePayloadMapper, IConfigurationModelRegistry configSetRegistry, IConfigurationValidator validator, IConfigRepository configRepository, IEventService eventService)
        {
            this.configurationService = configurationService;
            this.configurationUpdatePayloadMapper = configurationUpdatePayloadMapper;
            this.configSetRegistry = configSetRegistry;
            this.configRepository = configRepository;
            this.eventService = eventService;
            this.validator = validator;
        }

        public async Task<CommandResult> Handle(UpdateConfigurationFromEditorCommand command)
        {
            var instance = await configurationService.GetAsync(command.ConfigurationType, command.Identity);
            var configSetModel = configSetRegistry.GetConfigSetForConfig(command.ConfigurationType);
            ConfigInstance updatedInstance;
            try
            {
                updatedInstance = await configurationUpdatePayloadMapper.UpdateConfigurationInstance(instance, command.ConfigurationAsJson, configSetModel);
            }
            catch (ConfigModelParsingException ex)
            {
                return CommandResult.Failure(ex.Message);
            }
            var validationResult = await validator.Validate(updatedInstance.GetConfiguration(), configSetModel.Get(command.ConfigurationType),command.Identity);
            if (!validationResult.IsValid)
                return CommandResult.Failure(string.Join(Environment.NewLine, validationResult.Errors));
            await configRepository.UpdateConfigAsync(updatedInstance);
            await eventService.Publish(new ConfigurationUpdatedEvent(updatedInstance));
            return CommandResult.Success();
        }
    }
}
