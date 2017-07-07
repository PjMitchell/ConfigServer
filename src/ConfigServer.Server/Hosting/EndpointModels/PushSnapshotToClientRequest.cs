using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class PushSnapshotToClientRequest
    {
        public string[] ConfigsToCopy { get; set; }
    }
}
