using System;
using System.Collections.Generic;
using System.Text;
using ConfigServer.Server.Validation;
using ConfigServer.Core;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace ConfigServer.Server
{
    internal class UpdateConfigurationSetFromJsonUploadCommandHandler : ICommandHandler<UpdateConfigurationSetFromJsonUploadCommand>
    {
        private readonly IConfigurationModelRegistry registry;
        private readonly IConfigurationSetUploadMapper configurationSetUploadMapper;
        private readonly IConfigRepository configRepository;
        private readonly IConfigurationValidator configurationValidator;
        private readonly IEventService eventService;

        public UpdateConfigurationSetFromJsonUploadCommandHandler(IConfigurationModelRegistry registry, IConfigurationSetUploadMapper configurationSetUploadMapper, IConfigRepository configRepository, IConfigurationValidator configurationValidator, IEventService eventService)
        {
            this.registry = registry;
            this.configurationSetUploadMapper = configurationSetUploadMapper;
            this.configRepository = configRepository;
            this.configurationValidator = configurationValidator;
            this.eventService = eventService;
        }

        public async Task<CommandResult> Handle(UpdateConfigurationSetFromJsonUploadCommand command)
        {
            var configSetModel = registry.GetConfigSetDefinition(command.ConfigurationSetType);
            var mappedConfigs = configurationSetUploadMapper.MapConfigurationSetUpload(command.JsonUpload, configSetModel).ToArray();
            var propertyModelLookup = configSetModel.Configs.ToDictionary(k => k.Name, StringComparer.OrdinalIgnoreCase);
            var validationResult = await ValidateConfigs(mappedConfigs, propertyModelLookup, command.Identity);
            if (validationResult.IsValid)
            {
                foreach (var config in mappedConfigs)
                {
                    var model = propertyModelLookup[config.Key];
                    if (model.IsReadOnly)
                        continue;
                    ConfigInstance instance = BuildConfigInstance(command, model, config.Value);
                    await configRepository.UpdateConfigAsync(instance);
                    await eventService.Publish(new ConfigurationUpdatedEvent(instance));
                }
                return CommandResult.Success();
            }
            else
            {
                return CommandResult.Failure(string.Join(Environment.NewLine, validationResult.Errors));
            }
        }

        private static ConfigInstance BuildConfigInstance(UpdateConfigurationSetFromJsonUploadCommand command, ConfigurationModel model, object config)
        {
            ConfigInstance instance;
            if (model is ConfigurationOptionModel)
                instance = ConfigFactory.CreateGenericCollectionInstance(model.Type, command.Identity);
            else
                instance = ConfigFactory.CreateGenericInstance(model.Type, command.Identity);
            instance.SetConfiguration(config);
            return instance;
        }

        private async Task<ValidationResult> ValidateConfigs(IEnumerable<KeyValuePair<string, object>> source, Dictionary<string, ConfigurationModel> configSetModelLookup, ConfigurationIdentity configIdentity)
        {
            var validationResults = source.Select(async kvp => await configurationValidator.Validate(kvp.Value, configSetModelLookup[kvp.Key], configIdentity)).ToArray();
            await Task.WhenAll(validationResults);
            return new ValidationResult(validationResults.Select(s => s.Result));
        }

        private ConfigurationModel GetConfigurationSetForModel(ConfigInstance configInstance)
        {
            return registry.GetConfigDefinition(configInstance.ConfigType);
        }
    }
}
