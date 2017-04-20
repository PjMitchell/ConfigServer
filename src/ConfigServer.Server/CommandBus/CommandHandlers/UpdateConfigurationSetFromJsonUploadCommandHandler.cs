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
        private readonly IConfigurationSetRegistry registry;
        private readonly IConfigurationSetUploadMapper configurationSetUploadMapper;
        private readonly IConfigRepository configRepository;
        private readonly IConfigurationValidator configurationValidator;
        private readonly IEventService eventService;

        public UpdateConfigurationSetFromJsonUploadCommandHandler(IConfigurationSetRegistry registry, IConfigurationSetUploadMapper configurationSetUploadMapper, IConfigRepository configRepository, IConfigurationValidator configurationValidator, IEventService eventService)
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

        public static object GetObjectFromJsonOrDefault(string json, Type type)
        {
            bool failed = false;
            var result = JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings
            {
                Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
                {
                    failed = true;
                    args.ErrorContext.Handled = true;
                }
            });
            return failed ? null : result;
        }
    }
}
