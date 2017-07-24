using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class DeleteSnapshotCommand : ICommand
    {
        public string SnapshotId { get; set; }
        public string CommandName => nameof(DeleteSnapshotCommand);
    }
}
