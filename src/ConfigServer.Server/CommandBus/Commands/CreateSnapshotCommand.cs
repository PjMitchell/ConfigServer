using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class CreateSnapshotCommand : ICommand
    {
        public string ClientId { get; set; }
        public string Name { get; set; }

        public string CommandName => nameof(CreateSnapshotCommand);
    }
}
