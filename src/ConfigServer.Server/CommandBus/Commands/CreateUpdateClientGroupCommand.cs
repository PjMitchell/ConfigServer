using ConfigServer.Core;

namespace ConfigServer.Server
{
    internal class CreateUpdateClientGroupCommand : ICommand
    {
        public CreateUpdateClientGroupCommand(ConfigurationClientGroup clientGroup)
        {
            ClientGroup = clientGroup;
        }
        public string CommandName => nameof(CreateUpdateClientGroupCommand);
        public ConfigurationClientGroup ClientGroup { get; }
    }
}
