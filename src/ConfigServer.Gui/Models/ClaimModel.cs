using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigServer.Gui.Models
{
    public class UserClaimModel
    {
        public string Username { get; set; }
        public string ClientAdmin { get; set; }
        public string ReadClaims { get; set; }
        public string WriteClaims { get; set; }
    }

    public class ClaimModel
    {
        public string Type { get; set; }
        public string Value { get; set; }

    }
}
