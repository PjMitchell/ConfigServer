using System;
using ConfigServer.Core;
namespace ConfigServer.Server
{
    internal class UpdateConfigurationSetFromJsonUploadCommand : ICommand
    {
        public UpdateConfigurationSetFromJsonUploadCommand(ConfigurationIdentity identity, Type configurationSetType, string config)
        {
            Identity = identity;
            ConfigurationSetType = configurationSetType;
            JsonUpload = config;
        }

        public string CommandName => nameof(UpdateConfigurationSetFromJsonUploadCommand);
        public ConfigurationIdentity Identity { get; }
        public Type ConfigurationSetType { get; }
        public string JsonUpload { get; }
    }
}
