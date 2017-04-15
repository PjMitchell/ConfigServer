using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class CreateUpdateClientCommand : ICommand
    {
        public CreateUpdateClientCommand(ConfigurationClientPayload client)
        {
            Client = client;
        }

        public string CommandName => nameof(CreateUpdateClientCommand);

        public ConfigurationClientPayload Client { get; }
    }
}
