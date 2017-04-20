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
    internal class UpdateConfigurationFromJsonUploadCommandHandler : ICommandHandler<UpdateConfigurationFromJsonUploadCommand>
    {
        private readonly IConfigurationSetRegistry registry;
        private readonly IConfigurationService configurationService;
        private readonly IConfigRepository configRepository;
        private readonly IConfigurationValidator configurationValidator;
        private readonly IEventService eventService;

        public UpdateConfigurationFromJsonUploadCommandHandler(IConfigurationSetRegistry registry, IConfigurationService configurationService,IConfigRepository configRepository, IConfigurationValidator configurationValidator, IEventService eventService)
        {
            this.registry = registry;
            this.configurationService = configurationService;
            this.configRepository = configRepository;
            this.configurationValidator = configurationValidator;
            this.eventService = eventService;
        }

        public async Task<CommandResult> Handle(UpdateConfigurationFromJsonUploadCommand command)
        {
            var configInstance = await configurationService.GetAsync(command.ConfigurationType, command.Identity);
            var configType = configInstance is ConfigCollectionInstance
                ? ReflectionHelpers.BuildGenericType(typeof(IEnumerable<>), configInstance.ConfigType)
                : configInstance.ConfigType;

            var input = GetObjectFromJsonOrDefault(command.JsonUpload, configType);
            var validationResult = await configurationValidator.Validate(input, GetConfigurationSetForModel(configInstance), configInstance.ConfigurationIdentity);
            if (validationResult.IsValid)
            {
                configInstance.SetConfiguration(input);
                await configRepository.UpdateConfigAsync(configInstance);
                await eventService.Publish(new ConfigurationUpdatedEvent(configInstance));
                return CommandResult.Success();
            }
            else
            {
                return CommandResult.Failure(string.Join(Environment.NewLine, validationResult.Errors));
            }
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
