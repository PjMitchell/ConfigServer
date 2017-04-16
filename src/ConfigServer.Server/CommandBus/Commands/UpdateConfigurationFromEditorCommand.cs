using System;
using ConfigServer.Core;
namespace ConfigServer.Server
{
    internal class UpdateConfigurationFromEditorCommand : ICommand
    {
        public UpdateConfigurationFromEditorCommand(ConfigurationIdentity identity, Type configurationType, string config)
        {
            Identity = identity;
            ConfigurationType = configurationType;
            ConfigurationAsJson = config;
        }

        public string CommandName => nameof(UpdateConfigurationFromEditorCommand);
        public ConfigurationIdentity Identity { get; }
        public Type ConfigurationType { get; }
        public string ConfigurationAsJson { get; }
    }
}
