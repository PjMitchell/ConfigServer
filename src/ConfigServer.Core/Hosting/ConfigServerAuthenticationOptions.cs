using System;
using System.Security.Claims;

namespace ConfigServer.Core
{
    public class ConfigServerAuthenticationOptions
    {
        public bool RequireAuthentication { get; set; } = true;
        public string RequiredRole { get; set; }
        public Predicate<Claim> RequiredClaim { get; set; }
    }
}
