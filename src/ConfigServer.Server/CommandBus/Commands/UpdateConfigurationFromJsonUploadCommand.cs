using System;
using ConfigServer.Core;
namespace ConfigServer.Server
{
    internal class UpdateConfigurationFromJsonUploadCommand : ICommand
    {
        public UpdateConfigurationFromJsonUploadCommand(ConfigurationIdentity identity, Type configurationType, string config)
        {
            Identity = identity;
            ConfigurationType = configurationType;
            JsonUpload = config;
        }

        public string CommandName => nameof(UpdateConfigurationFromJsonUploadCommand);
        public ConfigurationIdentity Identity { get; }
        public Type ConfigurationType { get; }
        public string JsonUpload { get; }
    }
}
