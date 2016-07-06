using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ConfigServer.Core.Hosting
{
    public class ConfigServerAuthenticationOptions
    {
        public bool RequireAuthentication { get; set; } = true;
        public string RequiredRole { get; set; }
        public Predicate<Claim> RequiredClaim { get; set; }
    }
}
