﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigServer.Server
{
    internal class UserClientPermissions
    {
        public bool CanAccessClientAdmin { get; set; }
        public bool CanEditClients { get; set; }
        public bool CanEditGroups { get; set; }
        public bool CanDeleteArchives { get; set; }
        public bool HasClientConfiguratorClaim { get; set; }
    }
}
