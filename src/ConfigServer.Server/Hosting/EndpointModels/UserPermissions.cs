using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class UserPermissions
    {
        public bool CanAccessClientAdmin { get; set; }
        public bool CanAddClients { get; set; }
        public bool CanAddGroups { get; set; }
    }
}
