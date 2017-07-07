using ConfigServer.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class PushSnapshotToClientCommand : ICommand
    {
        public string SnapshotId { get; set; }
        public ConfigurationClient TargetClient { get; set; }
        public string[] ConfigsToCopy { get; set; }

        public string CommandName => nameof(PushSnapshotToClientCommand);
    }
}
